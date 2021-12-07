using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DFGE_lambda
{
    public class S3ObjectsManager
    {
        private readonly AmazonS3Client _s3Client;
        private readonly BasicAWSCredentials _awsCredentials;
        protected RegionEndpoint AwsRegion {get; set;}
        protected string BucketName { get; set; }

        public S3ObjectsManager()
        {

        }

        public S3ObjectsManager(string awsKey, string awsSecret, RegionEndpoint awsRegion, string bucketName)
        {
            AwsRegion = awsRegion;
            _awsCredentials = new BasicAWSCredentials(awsKey, awsSecret);
            _s3Client = new AmazonS3Client(_awsCredentials, awsRegion);
            BucketName = bucketName;



        }

        public async Task<Dictionary<S3Object, string>> CustomerFolderDictionary(string prefix)
        {
            Dictionary<S3Object, string> s3FileDictionary = new Dictionary<S3Object, string>();
            if (prefix == null || prefix == "")
            {
                throw new Exception("Empty prefix");
            }
            var listObjectsV2Paginator = _s3Client.Paginators.ListObjectsV2(new ListObjectsV2Request
            {
                BucketName = BucketName,
                Prefix = prefix
            });
            await foreach (var s3Object in listObjectsV2Paginator.S3Objects)
            {
                string objectFolderLevelCheck = s3Object.Key.Replace(prefix, string.Empty);
                int slashCount = objectFolderLevelCheck.Count(x => x == '/');
                bool isLastCharSlash = objectFolderLevelCheck.EndsWith('/');
                if ((slashCount == 1 && isLastCharSlash) || (slashCount == 0 && objectFolderLevelCheck.Length > 0))
                {
                    string pattern = @".*\/";
                    Match matchedStr = Regex.Match(s3Object.Key, pattern);
                    if (!isLastCharSlash)
                    {
                        s3Object.Key = s3Object.Key.Replace(matchedStr.ToString(), string.Empty);
                        s3FileDictionary.Add(s3Object, matchedStr.ToString());
                    }
                    else
                    {
                        var splitFolder = s3Object.Key.Split("/", StringSplitOptions.RemoveEmptyEntries);
                        var originalPrefix = s3Object.Key;
                        s3Object.Key = splitFolder.Last() + "/";
                        s3FileDictionary.Add(s3Object, originalPrefix);
                    }
                }
            }

            return s3FileDictionary;
        }







    }
}
