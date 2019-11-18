<!-- markdownlint-disable no-duplicate-heading -->
# Project Venimus - Public APIs

The following APIs can be called by unauthenticated users.

## GET /public/Groups

Returns a list of groups which are marked as active within the community.  The properties of a group are

* __Slug__ - The unique external ID for the group.  _For example _YorkCodeDojo_

* __Name__ - The unique name for the group / community.  _For example York Code Dojo_

* __Description__ - A description of the group in markdown

* __Logo__ - The URL of the group's logo.

---

## Get /public/FutureEvents

Returns a list of events which have been scheduled for the groups.  (A maximum of 10 per group).  The properties of an event are

* __EventSlug__ - The unique external ID for the event.

* __GroupName__ - The name of the group hosting the event.  _For example YorkCodeDojo_

* __EventTitle__ - The title of the event.  _For example Monthly meeting - October_

* __EventDescription__ - A description of the event in markdown.

* __EventStartsUTC__ - When does the event start,  in UTC time?

* __EventFinishesUTC__ - When does the event finish, in UTC time?

