<!-- markdownlint-disable no-duplicate-heading -->
# Project Venimus - APIs For Group Administrators

The following APIs can only be called by community group administrators.

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

* If the group does not exist then 404 will be returned. (NEEDS TEST)

* If the user is not an administrator for the group then Forbidden will be returned.

* The slug is required,  cannot contain spaces, must be unique for the group and no more than 100 characters. (NEEDS TEST)

* The start time must be in the future. (NEEDS TEST)

* The end time must be after the start time (NEEDS TEST)
