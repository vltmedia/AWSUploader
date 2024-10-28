# AWS S3 Uploader

This is a C# console application that uploads files and directories to an Amazon S3 bucket. The app uses AWS SDK for .NET to handle uploads to S3 and supports both single file and directory uploads. The program is configured through command-line arguments, including AWS credentials and target bucket details.

## Features

- Uploads single files or entire directories to S3.
- Configurable file/directory paths and target S3 locations.
- Supports public or private file upload settings.
- Region-specific bucket uploads.
- Reads multiple items for upload from a JSON file.

## Prerequisites

- [.NET 5.0 SDK or later](https://dotnet.microsoft.com/download)
- AWS SDK for .NET (`AWSSDK.S3` via NuGet)
- `Newtonsoft.Json` library for JSON handling.

## Setup and Usage

### Command-Line Arguments

The app accepts the following command-line arguments:

| Argument            | Description                                             |
| ------------------- | ------------------------------------------------------- |
| `--accesskey`       | AWS Access Key ID for authentication                    |
| `--secretkey`       | AWS Secret Access Key for authentication                |
| `--region`          | AWS Region of the S3 bucket (e.g., `us-east-1`)         |
| `--directory`       | Local directory path to upload                          |
| `--targetDirectory` | Target directory path in S3                             |
| `--bucketName`      | Name of the S3 bucket to upload to                      |
| `--public`          | Sets the uploaded file as public (default is private)   |
| `--file`            | Local file path to upload                               |
| `--targetFile`      | Target file path in S3                                  |
| `--data`            | Path to a JSON file with multiple upload configurations |

### Example Usage

1. **Upload a Directory**:
   
   ```sh
   AWSUploader --accesskey "YourAccessKey" --secretkey "YourSecretKey" --region "us-east-1" --directory "C:\Path\To\Directory" --targetDirectory "s3/path/in/bucket" --bucketName "your-bucket-name"
   ```

2. **Upload a Single File as Public**:
   
   ```sh
   AWSUploader --accesskey "YourAccessKey" --secretkey "YourSecretKey" --region "us-east-1" --file "C:\Path\To\File.txt" --targetFile "s3/path/in/bucket/File.txt" --bucketName "your-bucket-name" --public
   ```

3. **Upload Multiple Files and Directories from a JSON Configuration**:
   
   ```sh
   AWSUploader --accesskey "YourAccessKey" --secretkey "YourSecretKey" --region "us-east-1" --data "path/to/upload-config.json" --bucketName "your-bucket-name"
   ```
4. **Use Predefined Authentication Configuration**:
   ```sh
   AWSUploader --config "awsup_customuser_.json" --data "path/to/upload-config.json" --bucketName "your-bucket-name"
   ```
5. **Save To Configuration File For Later Use**:
    1. Does not process data, just saves the arguments to a file.
   ```sh
   AWSUploader --config "awsup_customuser_.json" --data "path/to/upload-config.json" --bucketName "your-bucket-name" --function save --savePath awsup_customuser_2.json
   ``` 

### JSON Configuration File Format

If using the `--data` argument, the configuration file should be in the following JSON format:

```json
{
  "files": [
    {
      "path": "C:/Path/To/File1.txt",
      "targetPath": "s3/path/in/bucket/File1.txt",
      "isPublic": true
    },
    {
      "path": "C:/Path/To/File2.txt",
      "targetPath": "s3/path/in/bucket/File2.txt",
      "isPublic": false
    }
  ],
  "directories": [
    {
      "path": "C:/Path/To/Directory1",
      "targetPath": "s3/path/in/bucket/Directory1",
      "isPublic": true
    }
  ]
}
```

### Building and Running

1. **Install Dependencies**: Ensure the AWS SDK and Newtonsoft.Json are installed.
   
   ```sh
   dotnet add package AWSSDK.S3
   dotnet add package Newtonsoft.Json
   ```

2. **Build the Project**:
   
   ```sh
   dotnet build
   ```

3. **Run the Project**:
   
   ```sh
   dotnet run -- [arguments]
   ```

## Code Overview

### Classes and Methods

- **`S3DirectoryUploader`**: Uploads an entire directory to a specified S3 path.
  
  - **`UploadDirectoryAsync`**: Asynchronously uploads each file in the local directory to the S3 bucket, preserving relative paths.

- **`S3FileUploader`**: Uploads a single file to S3.
  
  - **`UploadFileAsync`**: Asynchronously uploads a single file to the specified S3 bucket path.

- **`S3FilesUploader`**: Handles both file and directory uploads based on the arguments.
  
  - **`UploadDataAsync`**: Manages multi-file and directory uploads based on JSON configurations.

- **`AWSUploaderArguments`**: Parses command-line arguments and stores configurations, including access credentials and target paths.

- **`ApplicationRunner`**: Initializes `S3FilesUploader` with command-line arguments and runs the upload based on user input.

## Notes

- **Security**: Avoid hardcoding AWS credentials. Consider using environment variables or IAM roles in a production environment.
- **Logging**: Each upload action is logged to the console for easier tracking.
- **Error Handling**: Basic error handling is implemented for file operations and S3 connectivity.

---

Enjoy using the AWS S3 Uploader for convenient and efficient file and directory uploads to your S3 bucket!
