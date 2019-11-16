<!-- markdownlint-disable no-duplicate-heading -->
# Project Venimus - APIs For Group Members

The following APIs can only be called by members of the groups.

---

## POST /api/Groups/{GroupSlug}/events

Schedules a new event for a group.  The following needs to be provided.

* __Slug__ - The unique external ID for the event. _For example Nov2019_

* __Title__ - The title of the event.

* __Description__ - The full description of the event in markdown format.

* __Location__ - Where will the event be held?

* __StartTime__  - When will the event start?

* __EndTime__ - When will the event end?

### Business Rules

* If the event is successfully created then a URL to the event will be returned.

* If the group does not exist then 404 will be returned.

* If the user is not an administrator for the group then Forbidden will be returned.

* The slug is required,  cannot contain spaces, must be unique for the group and no more than 100 characters.

* The start time must be in the future.

* The end time must be after the start time.

---

## PUT /api/groups/{GroupSlug}/events/{EventSlug}

Allows a group administrator to update the details of an event.

The following needs to be provided.

* __Slug__ - The unique external ID for the event. _For example Nov2019_

* __Title__ - The title of the event.

* __Description__ - The full description of the event in markdown format.

* __Location__ - Where will the event be held?

* __StartTime__  - When will the event start?

* __EndTime__ - When will the event end?

### Business Rules

* If the group or event does not exist then 404 will be returned.

* If the user is not an administrator for the group then Forbidden will be returned.

* The slug is required,  cannot contain spaces, must be unique for the group and no more than 100 characters. 

* The end time must be after the start time.

### Note to developers

* The UI should warn the user if the event is in the past.  The API will allow it as the user could be correcting the details of an event which has already happened.

---

### POST /api/Groups/{GroupSlug}/ApprovedMembers

Group adminstrators can approve new members to the group. The following properties must be supplied:

* __UserSlug__ - The external ID for the user.

### Business Rules

* The user must be either a sysadmin administrator or an approved member of the group to be able to approve members.

* 404 is returned if the group does not exist.

* 400 is returned if the user is not a member of the group

* 400 is returned if the user is already an approved member of the group.

---

## GET /api/groups/{GroupSlug}/Members

Retrieves the list of members of the supplied group.  For the following array of properties are returned:

* __Slug__ - The external ID for the user.

* __IsAdministrator__ - Is the user a group administrator?

* __IsApproved__ - Has the user's group membership been approved?

* __EmailAddress__ - The email address which also links all the social media accounts together.

* __Pronoun__ - The users preferred personal pronon.  e.g. Him

* __Fullname__ - The user's fullname.  e.g David Betteridge

* __DisplayName__ - The user's name within the system.  Ideally the same as their slack name.  e.g. DavidB

* __Bio__ - The user's biography.  This can include their place of work/student,  any interests etc.

* __ProfilePictureInBase64__ - The user's profile picture

### Business Rules

* The user must be either a sysadmin administrator or an approved member of the group to retrieve the members list.

* 404 is returned if the group does not exist.

---

## GET /api/groups/{GroupSlug}/Events/{EventSlug}/Members

Retrieves the list of signed up attendees of the supplied event.  For the following array of properties are returned:

* __Slug__ - The external ID for the user.

* __IsHost__ - Is the user hosting the event?

* __IsSpeaker__ - Is the user speaking at the event?

* __EmailAddress__ - The email address which also links all the social media accounts together.

* __Pronoun__ - The users preferred personal pronon.  e.g. Him

* __Fullname__ - The user's fullname.  e.g David Betteridge

* __DisplayName__ - The user's name within the system.  Ideally the same as their slack name.  e.g. DavidB

* __Bio__ - The user's biography.  This can include their place of work/student,  any interests etc.

* __ProfilePictureInBase64__ - The user's profile picture

### Business Rules

* The user must be either a sysadmin administrator or an approved member of the group to retrieve the members list.

* 404 is returned if the group or event does not exist.

---

## DELETE /api/groups/{GroupSlug}/events/{EventSlug}

Allows an administrator to delete an event.

### Business Rules

* The user must be either a sysadmin administrator or an administrator of the group to delete the event.

* 204 is returned if event is successfully deleted.

* 204 is returned if the event does not exist.

* 404 is returned if the group does not exist.

* An event cannot be delete if it has already taken place.

---

## GET /api/groups/{GroupSlug}/events/{EventSlug}

Allows a member of a group to retrieve the full details of an event.  The following details are returned:

* __EventSlug__ - The unique external ID for the event.

* __GroupName__ - The name of the group hosting the event.  _For example YorkCodeDojo_

* __EventTitle__ - The title of the event.  _For example Monthly meeting - October_

* __EventDescription__ - A description of the event in markdown.

* __EventStartsUTC__ - When does the event start,  in UTC time?

* __EventFinishesUTC__ - When does the event finish, in UTC time?

* __EventLocation__ - Where will the event take place?

### Business Rules

* The user must be either a system administrator or a member of the group

* If the event or group does not exist then 404 will be returned.
