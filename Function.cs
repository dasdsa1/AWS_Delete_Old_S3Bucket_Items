using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using MySql.Data.MySqlClient;



// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DFGE_lambda
{
    public class Function
    {
        private readonly AmazonS3Client _s3Client = new AmazonS3Client();
        //private readonly ApplicationDbContext _dbContext;
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// 
        //private Function(ApplicationDbContext dbContext)
        //{
        //    _dbContext = dbContext;
        //}

        public async Task FunctionHandler(object input, ILambdaContext context)
        {
            //var folderList = _dbContext.Customer.FindAsync();
            context.Logger.Log("initializing \n");

            string server = Environment.GetEnvironmentVariable("DB_ENDPOINT") ?? "DB_ENDPOINT_STRING";
            string database = Environment.GetEnvironmentVariable("DATABASE") ?? "Climate_Platform_Test";
            string username = Environment.GetEnvironmentVariable("USER") ?? "USERID";
            string pwd = Environment.GetEnvironmentVariable("PASSWORD") ?? "PASSWORD";
            //string ConnectionString = String.Format("Server={0};Initial Catalog={1};User ID={2};Password={3}", server, database, username, pwd);
            string ConnectionString = $"server = {server}; port=1870; database = {database}; Uid ={username}; Pwd={pwd}";
            
            List<string> recyclePathList = new List<string>();

            try
            {
                using (var Conn = new MySqlConnection(ConnectionString))
                {
                    using (var Cmd = new MySqlCommand($"SELECT * FROM Climate_Platform_Test.Customer;", Conn))
                    {

                        Conn.Open();

                        MySqlDataReader rdr = Cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            recyclePathList.Add(rdr[11].ToString());
                        }
                        

                    }
                }
            }
            catch(Exception ex)
            {
                context.Logger.Log(ex.Message);
            }


            int policyDays = -90;
            
            

            var bucketName = "BUCKET_NAME";

            S3ObjectsManager s3ObjectsManager = new S3ObjectsManager("AWS_KEY", "AWS_SECRET", RegionEndpoint.EUCentral1, bucketName);
            //load all S3RecycleBin path for customers.

            List<DeleteObjectRequest> deletedItemsList = new List<DeleteObjectRequest>();

            foreach (var recyclePath in recyclePathList)
            {
                Dictionary<S3Object, string> myDictionary = new Dictionary<S3Object, string>();
                myDictionary = await s3ObjectsManager.CustomerFolderDictionary(recyclePath);

                context.Logger.Log(myDictionary.Count().ToString());
                foreach (var dictionaryItem in myDictionary)
                {
                    //context.Logger.Log($"ItemValue: {dictionaryItem.Value}, KeyKey/FileName: {dictionaryItem.Key.Key}  ");
                    if (dictionaryItem.Key.LastModified < DateTime.Now.AddDays(policyDays))
                    {

                        context.Logger.Log(dictionaryItem.Key.Key + " Being processed.\n");
                        DeleteObjectRequest deleteReqFile = new DeleteObjectRequest()
                        {
                            BucketName = bucketName,
                            Key = recyclePath + dictionaryItem.Key.Key
                        };

                        


                        //await _s3Client.DeleteObjectAsync(deleteReqFile);
                        deletedItemsList.Add(deleteReqFile);
                        context.Logger.Log(dictionaryItem.Key.Key + " Finished.\n");
                    }
                    else
                    {
                        context.Logger.Log(dictionaryItem.Key.Key + " Not deleted \n");
                    }
                }
            }



            //string dbServer = Environment.GetEnvironmentVariable("DB_ENDPOINT") ?? "platform-dfge.cgdg3povuypd.eu-central-1.rds.amazonaws.com";
            //string dbServer = "127.0.0.1";
            string dbDatabase = Environment.GetEnvironmentVariable("DATABASE") ?? "Climate_Platform_Test";
            string dbUsername = Environment.GetEnvironmentVariable("USER") ?? "USER_ID";
            string dbPwd = Environment.GetEnvironmentVariable("PASSWORD") ?? "PASSWORD";
            string dbConnectionString = String.Format("Server={0};Initial Catalog={1};User ID={2};Password={3}", server, database, username, pwd);
            //string dbConnectionString = "server=localhost; port=3306; database=climate_platform_hometest; Uid=root; Pwd=K5CoreTest";//$"server = {dbServer}; port=3306; database = {dbDatabase}; Uid ={dbUsername}; Pwd={dbPwd}";

            try
            {
                using (var Conn = new MySqlConnection(ConnectionString))
                {
                    
                    
                    foreach (var deletedItem in deletedItemsList)
                    {
                        context.Logger.Log(deletedItem.Key + "\n Starting...");
                        AppActivityLog appActivityLog = new AppActivityLog()
                        {
                            entry_data_action = "Delete",
                            user_id = "SYSTEM LAMBDA",
                            controller = "n/a",
                            entry_date = DateTime.Now,
                            entry_data = deletedItem.Key,
                            entry_data_action_message = "Lambda S3",
                            customer_id = 0

                        };
                        context.Logger.Log(deletedItem.Key + "\n Connecting...");
                        using (var Cmd = new MySqlCommand($"INSERT INTO AppActivityLog (entry_data_action, user_id," +
                            $" controller, entry_date, entry_data, entry_data_action_message, customer_id)" +
                            $" VALUES ('{appActivityLog.entry_data_action}'," +
                            $" '{appActivityLog.user_id}', '{appActivityLog.controller}', (STR_TO_DATE('{appActivityLog.entry_date}','%d/%m/%Y %H:%i:%s')), '{appActivityLog.entry_data}'," +
                            $" '{appActivityLog.entry_data_action_message}', '{appActivityLog.customer_id}');", Conn))
                        {
                            context.Logger.Log(deletedItem.Key + "\n Writing...\n");
                            Conn.Open();
                            MySqlDataReader rdr = Cmd.ExecuteReader();
                            context.Logger.Log(deletedItem.Key + "\n Ending.");
                        }

                    }
                    
                    
                }
            }
            catch(Exception ex)
            {
                context.Logger.Log(ex.Message);
            }



            
            


            context.Logger.Log("\nEnd");


        }
    }
}
