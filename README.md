<!-- markdownlint-disable no-duplicate-heading -->
# Project Venimus

Open source event organising community platform ran by https://YorkDevelopers.org

## Build Status

[![Build Status](https://dev.azure.com/david0415/Venimus/_apis/build/status/YorkDevelopers.venimus?branchName=master)](https://dev.azure.com/david0415/Venimus/_build/latest?definitionId=1&branchName=master)

## MongoDB

You will need to install an instance of MongoDB locally or sign up to a cloud provider such as Atlas

Then add/update the `appsettings.json` with your details

For example

```json
  "MongoDB": {
    "connectionString": "mongodb://localhost:27017",
    "databaseName": "venimus"
  }
```  

It is recommended that rather than editing the existing `appsettings.json` file you create `appsettings.Testing.json` and/or `appsettings.Development.json` files.

---

## Auth0

Authentication is handled by a 3rd party called Auth0.  Users are automatically created in Auth0 the first time they log in.

The granting of a user the right to be a system administrator is carried out in the Auth0 dashboard by manually granting the user the "System Administrator" role.

The unit tests used our mocked version of Auth0.  This is configured by adding the following settings to your `appsettings.json` file

```json
  "Auth0": {
    "Domain": "venimus-mockauth.azurewebsites.net",
    "Audience": "https://Venimus.YorkDevelopers.org"
  }
```  

It is recommended that rather than editing the existing `appsettings.json` file you create `appsettings.Testing.json` and/or `appsettings.Development.json` files.

---

## Available APIs

In addition to the generated swagger documentation,  a summary of the APIs is below.

[APIs for System Administrators](docs/APIs_for_SystemAdministrators.md)

[APIs for Group Administrators](docs/APIs_for_GroupAdministrators.md)

[APIs for Normal Users](docs/APIs_for_NormalUsers.md)

[APIs for Unauthenticated Users](docs/APIs_for_ThePublic.md)

---
