# Project Venimus
Open source event organising community platform ran by YorkDevelopers.org

## Build Status

[![Build Status](https://dev.azure.com/david0415/Venimus/_apis/build/status/YorkDevelopers.venimus?branchName=master)](https://dev.azure.com/david0415/Venimus/_build/latest?definitionId=1&branchName=master)

## Get Started

You will need to install an instance of MongoDB locally or sign up to a cloud provider such as Atlas

Then add/update the `appsettings.Testing.json` with your details

For example

```json
  "MongoDB": {
    "connectionString": "mongodb+srv://app:password@cluster0-mwrwp.azure.mongodb.net/test?retryWrites=true&w=majority",
    "databaseName": "venimus"
  }
```  


## APIS

System adminstrators can :  List / Create / Edit / Delete Groups / Ban Users

Group administrators can : List (including people) / Create / Edit / Delete Events

Users can : Signup / Join/Leave Groups / RSVP/SignDown to/from Events / View Groups / View Events / Change Details


