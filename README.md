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

## APIs For Logged In Users

The following APIs can be called by normal users once they have authenticated.

### GET /api/User/Groups

Returns the active groups that the current user is a member of.  The properties of a group are

* __Slug__ - The unique external ID for the group.  _For example _YorkCodeDojo_

* __Name__ - The unique name for the group / community.  _For example York Code Dojo_

* __Description__ - A description of the group in markdown

* __SlackChannelName__ - The name of this groups slack channel

* __LogoInBase64__ - The group's logo.

#### Business Rules

Only active groups are returned.

### POST /api/User/Groups

Allows the user to join a group.  The supplied model must contain:

* __GroupSlug__ - The unique external ID for the group. _For example YorkCodeDojo_

#### Business Rules

* If the user is successfully added to the group then 204 is returned.

* If the group does not exist then 404 is returned.  (NEEDS TEST)

* If the user is already a member of a group then 204 will still be returned. (NEEDS TEST)

### DELETE /api/User/Groups/{groupSlug}

Allows the user to leave a group.  

#### Business Rules

* If the user is successfully removed from the group then 204 is returned.
 
* If the group does not exist then 404 is returned. (NEEDS TEST)

* If the user is not a member of a group then 204 will still be returned.  (NEEDS TEST)

---

## APIs For System Administrators

The following APIs can only be called by sysadmin administrators.

### POST /api/Groups

Creates a new group.  The body of the request must include the following information.

* __Slug__ - The unique external ID for the group.  _For example _YorkCodeDojo_

* __IsActive__ - Is the community currently active.

* __Name__ - The unique name for the group / community.  _For example York Code Dojo_

* __Description__ - A description of the group in markdown

* __SlackChannelName__ - The name of this groups slack channel

* __LogoInBase64__ - The group's logo.

#### Business Rules

* The slug is required,  cannot contain spaces, must be unique and no more than 100 characters. (NEEDS TEST)

* The name is required, must be unique and no more than 100 characters. (NEEDS TEST)

* The description is required,  and should be in markdown format

### PUT /api/Groups/{slug}

Updates an existing group with the matching `slug`. The body of the request must include the following information.

* __Slug__ - The unique external ID for the group.  _For example _YorkCodeDojo_

* __IsActive__ - Is the community currently active.

* __Name__ - The unique name for the group / community.  _For example York Code Dojo_

* __Description__ - A description of the group in markdown

* __SlackChannelName__ - The name of this groups slack channel

* __LogoInBase64__ - The group's logo.

#### Business Rules

* The slug is required,  cannot contain spaces, must be unique and no more than 100 characters. (NEEDS TEST)

* The name is required, must be unique and no more than 100 characters. (NEEDS TEST)

* The description is required,  and should be in markdown format

---

TODO!

System adminstrators can :  Delete Groups / Ban Users
### DELETE /api/Groups/{groupslug}
### POST /api/BannedUsers/{userslug}
### DELETE /api/BannedUsers/{userslug}

Group administrators can : List (including people) / Create / Edit / Delete Events
### GET /api/Groups/{GroupSlug}/Members
### POST /api/Groups/{GroupSlug}/Events
### PUT /api/Groups/{GroupSlug}/Events
### DELETE /api/Groups/{GroupSlug}/Events

Users can : Signup /  RSVP/SignDown to/from Events / View Groups / View Events / Change Details
