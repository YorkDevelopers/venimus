# Project Venimus

Open source event organising community platform ran by YorkDevelopers.org

## Build Status

[![Build Status](https://dev.azure.com/david0415/Venimus/_apis/build/status/YorkDevelopers.venimus?branchName=master)](https://dev.azure.com/david0415/Venimus/_build/latest?definitionId=1&branchName=master)

## Get Started - MongoDB

You will need to install an instance of MongoDB locally or sign up to a cloud provider such as Atlas

Then add/update the `appsettings.json` with your details

For example

```json
  "MongoDB": {
    "connectionString": "mongodb://localhost:27017",
    "databaseName": "venimus"
  }
```  

You will also need to configure the Auth provider.  To use our mocked version add the following lines to your `appsettings.json` file

```json
  "Auth0": {
    "Domain": "venimus-mockauth.azurewebsites.net",
    "Audience": "https://Venimus.YorkDevelopers.org"
  }
```  

It is recommended that rather than editing the existing `appsettings.json` file you create `appsettings.Testing.json` and/or `appsettings.Development.json` files.

---

## Public APIs

The following APIs can be called by unauthenticated users.

### GET /public/Groups

Returns a list of groups which are marked as active within the community.  The properties of a group are

* __Slug__ - The unique external ID for the group.  _For example _YorkCodeDojo_

* __Name__ - The unique name for the group / community.  _For example York Code Dojo_

* __Description__ - A description of the group in markdown

* __SlackChannelName__ - The name of this groups slack channel

* __LogoInBase64__ - The group's logo.

### Get /public/FutureEvents

Returns a list of events which have been scheduled for the groups.  (A maximum of 10 per group).  The properties of an event are

* __EventSlug__ - The unique external ID for the event.

* __GroupName__ - The name of the group hosting the event.  _For example YorkCodeDojo_

* __EventTitle__ - The title of the event.  _For example Monthly meeting - October_

* __EventDescription__ - A description of the event in markdown.

* __EventStartsUTC__ - When does the event start,  in UTC time?

* __EventFinishesUTC__ - When does the event finish, in UTC time?

---

## Private APIs

System adminstrators can :  List / Create / Edit / Delete Groups / Ban Users

Group administrators can : List (including people) / Create / Edit / Delete Events

Users can : Signup / Join/Leave Groups / RSVP/SignDown to/from Events / View Groups / View Events / Change Details
