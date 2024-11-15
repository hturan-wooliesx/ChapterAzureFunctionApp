# ChapterAzureFunctionApp

This repository is created to demonstrate learnings out of chapter activity https://woolworthsdigital.atlassian.net/browse/GEC-14. It covers different type of Azure functions and usages in the same Azure Function App.

# Local Setup and Debugging
This solution uses Azurite for running local storage services for Azure Blob Containers, Queues and Tables. For setting up Azurite on your local and run it, please see https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio%2Cblob-storage.

Solution uses default ports for Azurite -> blobPort : 10000, queuePort: 10001 and tablePort : 10002

In order to manage resources that are created by Azurite and other Azure storage data, download Azure Storage Explorer on https://learn.microsoft.com/en-us/azure/storage/storage-explorer/vs-azure-tools-storage-manage-with-storage-explorer?tabs=windows

Due to an error in Azurite with connecting local queue storages or blob containers with connection strings, I have used dev environment Service bus instance to work with dummy queue named q2.

# Function Types
In the solution, common Azure function use cases and type of functions that are relevant for business use cases created. They are HTTP triggered, Timer triggered and Blob triggered functions. 
