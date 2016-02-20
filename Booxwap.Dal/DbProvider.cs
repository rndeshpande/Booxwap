namespace Booxwap.Dal
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using Interfaces;
    using Models;
    using Utilities;

    public class DbProvider : IDbProvider
    {
        public IDictionary<string, string> GetFoundBooksList(string bookTitle, string userId)
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("SL_Title", bookTitle),
                new SqlParameter("SL_UserID", userId),
            };
            var data = ExecuteSelectStoredProcedure(SpNames.FoundBooksDetails, parameters);
            return data.Rows.Cast<DataRow>().ToDictionary(row => row["WL_RowID"].ToString(), row => row["WL_Title"].ToString());
        }

        public IDictionary<string, string> GetWishList(string fbId)
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("UserId",fbId)
            };
            var data = ExecuteSelectStoredProcedure(SpNames.GetWishList, parameters);
            return data.Rows.Cast<DataRow>().ToDictionary(row => row["WL_RowID"].ToString(), row => row["WL_Title"].ToString());
        }

        public IList<MatchListModel> GetMatchingBooks(string userId)
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("UserId",userId)
            };

            var data = ExecuteSelectStoredProcedure(SpNames.GetMatchingBooks, parameters);

            return (from DataRow dr in data.Rows
                    select new MatchListModel
                    {
                        BookId = dr["SL_RowID"].ToString(),
                        Title = dr["SL_Title"].ToString(),
                        UserFacebookId = dr["User_FBID"].ToString(),
                        UserId = dr["SL_UserID"].ToString(),
                        UserFirstName = dr["User_FirstName"].ToString(),
                        UserLastName = dr["User_LastName"].ToString(),
                    }).ToList();
        }

        public IDictionary<string, string> GetShareList(string userId)
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("UserId",userId)
            };

            var data = ExecuteSelectStoredProcedure(SpNames.GetShareList, parameters);
            return data.Rows.Cast<DataRow>().ToDictionary(row => row["WL_RowID"].ToString(), row => row["WL_Title"].ToString());
        }

        public IList<NewsModel> GetNewsStream(string userId)
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("UserId",userId)
            };

            var data = ExecuteSelectStoredProcedure(SpNames.GetNewsStream, parameters);

            return (from DataRow dr in data.Rows
                    select new NewsModel
                    {
                        ListItemId = dr["ID"].ToString(),
                        ActionType = dr["Type"].ToString(),
                        ActionDate = dr["DateAdded"].ToString(),
                        UserFullName = dr["Username"].ToString(),
                        UserFacebookId = dr["User_FBID"].ToString(),
                        UserFacebookUrl = dr["FBProfile"].ToString(),
                        BookName = dr["BookName"].ToString(),
                        BookAsin = dr["ASIN"].ToString()
                    }).ToList();
        }

        public bool SaveReview(string bbid, string bookTitle, string author, string amazonUrl, string bookType, string content)
        {
            var parameters = new SqlParameter[]
                    {
                      new SqlParameter("@UserID", bbid),
                      new SqlParameter("@Title", bookTitle),
                      new SqlParameter("@Author", author),
                      new SqlParameter("@AmazonURL", amazonUrl),
                      new SqlParameter("@Type", bookType),
                      new SqlParameter("@Content", content)
                    };
            return ExecuteInsertStoredProcedure(SpNames.SaveReview, parameters) > 0;
        }

        public string UpdateUserInfo(string fbid, string firstName, string lastName, string fbProfileLink)
        {
            using (var sqlConn = new SqlConnection(ConfigurationProvider.ConnectionString))
            {
                var sqlCmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    Connection = sqlConn,
                    CommandText = SpNames.UpdateUserInfo
                };

                var parameters = new SqlParameter[]
                {
                    new SqlParameter( "User_FBID", fbid),
                    new SqlParameter("User_FirstName", firstName),
                    new SqlParameter("User_LastName", lastName),
                    new SqlParameter("User_FBProfileLink", fbProfileLink),
                    new SqlParameter("User_RowID", 0)
                    {
                        Direction = ParameterDirection.Output
                    }
                };

                sqlCmd.Parameters.AddRange(parameters);
                sqlConn.Open();
                sqlCmd.ExecuteNonQuery();

                var strUserRowId = sqlCmd.Parameters["User_RowID"].Value.ToString();
                return strUserRowId;
            }
        }

        public bool AddToDatabase(string bbid, string bookAsin, string mode)
        {
            try
            {
                var sqlConn = new SqlConnection(ConfigurationProvider.ConnectionString);
                var sqlCmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    Connection = sqlConn
                };

                switch (mode)
                {
                    case Constants.ListTypeShare:
                        sqlCmd.CommandText = SpNames.AddToShareListUsingAsin;
                        break;

                    case Constants.ListTypeWish:
                        sqlCmd.CommandText = SpNames.AddToWishListUsingAsin;
                        break;
                }

                sqlCmd.Parameters.AddWithValue("@UserID", bbid);
                sqlCmd.Parameters.AddWithValue("@ASIN", bookAsin);
                var sqlParam = new SqlParameter("@Result", 0)
                {
                    Direction = ParameterDirection.Output
                };
                sqlCmd.Parameters.Add(sqlParam);

                sqlConn.Open();
                sqlCmd.ExecuteNonQuery();
                sqlConn.Close();

                var strResult = sqlCmd.Parameters["@Result"].Value.ToString();
                return strResult == "1";
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool AddBookToList(string bbid, string bookTitle, string mode, string author, string amazonUrl, string bookType, string asin)
        {
            try
            {
                var sqlConn = new SqlConnection(ConfigurationProvider.ConnectionString);
                var sqlCmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    Connection = sqlConn
                };

                switch (mode)
                {
                    case Constants.ListTypeShare:
                        sqlCmd.CommandText = SpNames.AddToShareList;
                        break;

                    case Constants.ListTypeWish:
                        sqlCmd.CommandText = SpNames.AddToWishList;
                        break;
                }

                sqlCmd.Parameters.AddWithValue("@UserID", bbid);
                sqlCmd.Parameters.AddWithValue("@Title", bookTitle);
                sqlCmd.Parameters.AddWithValue("@Author", author);
                sqlCmd.Parameters.AddWithValue("@AmazonURL", amazonUrl);
                sqlCmd.Parameters.AddWithValue("@Type", bookType);
                sqlCmd.Parameters.AddWithValue("@ASIN", asin);
                var sqlParam = new SqlParameter("@Result", 0)
                {
                    Direction = ParameterDirection.Output
                };
                sqlCmd.Parameters.Add(sqlParam);

                sqlConn.Open();
                sqlCmd.ExecuteNonQuery();
                sqlConn.Close();

                var strResult = sqlCmd.Parameters["@Result"].Value.ToString();
                return strResult == "1";
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool SaveList(string bbid, string bookTitle, string mode, string author, string amazonUrl, string strBookType, string strAsin)
        {
            try
            {
                string strConnString = ConfigurationManager.AppSettings["ConnectionString"];

                using (var sqlConn = new SqlConnection(strConnString))
                {
                    var sqlCmd = new SqlCommand
                    {
                        CommandType = CommandType.StoredProcedure,
                        Connection = sqlConn
                    };

                    if (mode == "Share")
                        sqlCmd.CommandText = "AddToShareList";
                    else if (mode == "Wish")
                        sqlCmd.CommandText = "AddToWishList";

                    sqlCmd.Parameters.AddWithValue("@UserID", bbid);
                    sqlCmd.Parameters.AddWithValue("@Title", bookTitle);
                    sqlCmd.Parameters.AddWithValue("@Author", author);
                    sqlCmd.Parameters.AddWithValue("@AmazonURL", "");
                    sqlCmd.Parameters.AddWithValue("@Type", strBookType);
                    sqlCmd.Parameters.AddWithValue("@ASIN", strAsin);

                    var sqlParam = new SqlParameter("@Result", 0) { Direction = ParameterDirection.Output };
                    sqlCmd.Parameters.Add(sqlParam);

                    sqlConn.Open();
                    sqlCmd.ExecuteNonQuery();

                    var strResult = sqlCmd.Parameters["@Result"].Value.ToString();
                    if (strResult == "1")
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex.Message, ex.StackTrace);
                return false;
            }
        }

        #region Private Functions

        private static DataTable ExecuteSelectStoredProcedure(string spName, SqlParameter[] parameters)
        {
            using (var sqlConn = new SqlConnection(ConfigurationProvider.ConnectionString))
            {
                var sqlCmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    Connection = new SqlConnection(ConfigurationProvider.ConnectionString),
                    CommandText = spName
                };

                sqlCmd.Parameters.AddRange(parameters);

                var da = new SqlDataAdapter(sqlCmd);
                var dtData = new DataTable();

                da.Fill(dtData);

                return dtData;
            }
        }

        private static int ExecuteInsertStoredProcedure(string spName, SqlParameter[] parameters)
        {
            using (var sqlConn = new SqlConnection(ConfigurationProvider.ConnectionString))
            {
                var sqlCmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    Connection = new SqlConnection(ConfigurationProvider.ConnectionString),
                    CommandText = spName
                };

                sqlCmd.Parameters.AddRange(parameters);
                return sqlCmd.ExecuteNonQuery(); ;
            }
        }

        #endregion Private Functions
    }
}