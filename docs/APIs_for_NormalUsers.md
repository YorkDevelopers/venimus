<!-- markdownlint-disable no-duplicate-heading -->
# Project Venimus - Normal User APIs

The following APIs can be called by normal users once they have authenticated.

## GET /api/User

Allows a user to view their profile details.  The following properties are returned:

* __EmailAddress__ - The email address which also links all the social media accounts together.
* __Pronoun__ - The users preferred personal pronon.  e.g. Him
* __Fullname__ - The user's fullname.  e.g David Betteridge
* __DisplayName__ - The user's name within the system.  Ideally the same as their slack name.  e.g. DavidB   (Has to be unique)
* __Bio__ -  The user's biography.  This can include their place of work/student,  any interests etc. (Visible to all signed in members)
* __ProfilePictureAsBase64__ - The user's profile picture in base64

### Note to implementers

It is recommended that the user gets a chance to review their profile as part of the sign-up process.

---

## PUT /api/User

Allows a user to update their profile.  The following properties must be supplied

* __Pronoun__ - The users preferred personal pronon.  e.g. Him
* __Fullname__ - The user's fullname.  e.g David Betteridge
* __DisplayName__ - The user's name within the system.  Ideally the same as their slack name.  e.g. DavidB   (Has to be unique)
* __Bio__ -  The user's biography.  This can include their place of work/student,  any interests etc. (Visible to all signed in members)
* __ProfilePictureAsBase64__ - The user's profile picture in base64

### Business Rules

* The `DisplayName` must be unique.

---

## GET /api/user/events

Allows the user to view all the future events they have signed up to.  The following properties are returned:

* __GroupSlug__ - the ID of the group.
* __GroupName__ - the name of the group.
* __EventSlug__ - the ID of the event within the group.
* __EventTitle__ - the title of the event
* __EventDescription__ - the full description of the event in markdown
* __EventStartsUTC__ - Date and time the event starts
* __EventFinishesUTC__ - Date and time the event ends

---

## POST api/user/groups/{GroupSlug}/Events

Allows the user to sign up for an event.  The following details must be provided:

* __EventSlug__ - the ID of the event within the group.
* __NumberOfGuests__ - the number of unregistered guests
* __DietaryRequirements__ - any dietary requirements
* __MessageToOrganiser__ - free format message

### Business Rules

* Group does not exist
* Event does not belong to the group
* Event is in the past
* Event is full
* NumberOfGuests is positive
* Guests not allowed  
* User is not a member of the group
* A user can only sign up once.

---

## PUT api/user/groups/{GroupSlug}/Events/{EventSlug}

Allows the user to amend their registration for an event.  The following details must be provided:

* __NumberOfGuests__ - the number of unregistered guests
* __DietaryRequirements__ - any dietary requirements
* __MessageToOrganiser__ - free format message

### Business Rules

* Group does not exist
* Event does not belong to the group
* User is not a member of the group.
* Event is in the past
* NumberOfGuests is positive
* Guests not allowed
* The user isn't signed up then a record is created
* Too many people (**not implemented**)

---

## GET /api/user/groups/{GroupSlug}/Events/{EventSlug}

Allows the user to view their registration details for any event.  Returns the following properties

* NumberOfGuests - the number of unregistered guests
* DietaryRequirements - any dietary requirements
* MessageToOrganiser - free format message

### Business Rules

* Event does not exist
* Group does not exist
* Not a member of the group
* Not signed up (**not implemented**)

---

## DELETE /api/user/groups/{GroupSlug}/Events/{EventSlug}

Allows the user to record that they are no longer planning on attending an event.

### Business Rules

* This API can be called regardless of if the user is currently signed up to the event or not.

* If the event does not exist,  then 404 will be returned.

* If the event is in the past,  then an error will be returned.

---

## GET /api/User/Groups

Returns the active groups that the current user is a member of.  The properties of a group are

* __Slug__ - The unique external ID for the group.  _For example _YorkCodeDojo_

* __Name__ - The unique name for the group / community.  _For example York Code Dojo_

* __Description__ - A description of the group in markdown

* __SlackChannelName__ - The name of this groups slack channel

* __LogoInBase64__ - The group's logo.

### Business Rules

Only active groups are returned.

---

## GET /api/User/Groups/{GroupSlug}

Returns the details of a single group the user is a member of.  The properties of a group are

* __GroupSlug__ - The unique external ID for the group.  _For example _YorkCodeDojo_

* __GroupName__ - The unique name for the group / community.  _For example York Code Dojo_

* __GroupDescription__ - A description of the group in markdown

* __GroupSlackChannelName__ - The name of this groups slack channel

* __GroupLogoInBase64__ - The group's logo.

### Business Rules

If the user is not a member of the group or the group does not exist then 404 is returned.

---

## POST /api/User/Groups

Allows the user to join a group.  The supplied model must contain:

* __GroupSlug__ - The unique external ID for the group. _For example YorkCodeDojo_

### Business Rules

* If the user is successfully added to the group then 201 is returned with location to retrieve the groups details.

* If the group does not exist then 404 is returned.

* If the user is already a member of a group then 204 will still be returned but they will only be added once.

---

## DELETE /api/User/Groups/{groupSlug}

Allows the user to leave a group.  

### Business Rules

* If the user is successfully removed from the group then 204 is returned.

* If the group does not exist then 404 is returned.

* If the user is not a member of a group then 204 will still be returned.

---
