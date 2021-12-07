Simple (highly coupled) AWS Function in C#

    1-Read a table and get the 11th Index from it, representing a path and add it to a List.
    2-Retrieve a dictionary, from a S3_Bucket, with the files that match the strings in the List.
    3-Validate if the files in the Dictionary have the property "dictionaryItem.Key.LastModified" older than 90 days.
    4-If the files are older than 90 days, delete the files from the S3 Bucket and log it to a database.
    5-Else, skip.
    
