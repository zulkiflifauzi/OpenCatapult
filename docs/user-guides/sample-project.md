# Create Project by using Sample template

Sample Devops project template contains the following job tasks:

```
CLONE -> GENERATE --> PUSH -> MERGE --> BUILD --> DEPLOYDB --> DEPLOY
```

_CLONE_ task will clone the remote repository into the engine.

_GENERATE_ task will generate an ASP.NET Core MVC application.

_PUSH_ task will push the generated code to GitHub repository. It will also create a Pull Request so you can review it before merging it into the master branch

_Merge_ task will merge the generated Pull Request to the branch master of the GitHub repository

_BUILD_ task will build the code and produce an artifact.

_DEPLOYDB_ task will apply generated Entity Framework Core migrations into an SQL Server database.

_DEPLOY_ task will deploy the artifact to Azure App service.

## Pre-requisites

Before generating a project with Sample Devops template, you need to prepare some pre-requisites to make connection to external services.

**GitHub**

In order to connect to GitHub, you can choose to provide `username/password` or `personal access token`. If you choose the latest, you can find how to get it at https://help.github.com/articles/creating-a-personal-access-token-for-the-command-line/.

A Github repository will be created automatically by `OpenCatapult`.

**Azure App Service**

`OpenCatapult` will create Azure App Services automatically, but you still need to create a generic app service and make note of it's `Application ID`, `Authentication Key`, and `Tenant ID` values as an authentication to your Azure subscription. The `Tenant ID` is also referred to as the `Directory ID` on Azure. You can get them by following the steps explained at https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-create-service-principal-portal#get-application-id-and-authentication-key.

**SQL Server Database**

Basically you can host the database anywhere as long as it is reachable by the application. We recommend to use Azure SQL in the same region as the App Service to make them closer.

You need to create the database manually in the server for now. In the future, this procedure will be taken care automatically by `OpenCatapult`. Please make a note for the database connection string.

## Create required external services

After you have all of the required values above, next is to store them securely in one of `OpenCatapult` feature called `External Service`. By creating this, you don't have to enter sensitive information directly in Job Task definition. You just need to include the External Service name in the task, and let the system fetch the secret values in secure way.

Let's create an External Service for `GitHub`:

```sh
dotnet occli.dll service add --name github-default --type GitHub
```

Then you will be prompted to enter the required details for `GitHub`.

And now, let's create External Service for `Azure`:

```sh
dotnet occli.dll service add --name azure-default --type Azure
```

Then you will be prompted to enter the required details for `Azure`.

## Create project

After all required components in place, now you can start to create the project by using Sample Devops template. Please open a new publish CLI shell window

```sh
dotnet occli.dll project create --name SampleProject --client Polyrific --template sample-devops
```

You will be prompted to enter some additional configurations.

After your project is created, you can check what you have in it, e.g. models, jobs, etc.

To run the job queue, please make sure you have registered and started your engine. Please follow the link [here](../home/start.md#register-and-start-the-engine) for detail.

At this state, you should be able to direclty send the job from your created project to the queue:

```sh
dotnet occli.dll queue add --project SampleProject --job Default
```
