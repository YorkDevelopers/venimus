<!-- markdownlint-disable no-duplicate-heading -->
# Project Venimus - APIs For System Administrators

The following APIs can only be called by sysadmin administrators.

---

## POST /api/Groups

Creates a new group.  The body of the request must include the following information.

* __Slug__ - The unique external ID for the group.  _For example _YorkCodeDojo_

* __IsActive__ - Is the community currently active.

* __Name__ - The unique name for the group / community.  _For example York Code Dojo_

* __Description__ - A description of the group in markdown

* __SlackChannelName__ - The name of this groups slack channel

* __LogoInBase64__ - The group's logo.

### Business Rules

* The slug is required,  cannot contain spaces, must be unique and no more than 100 characters.

* The name is required, must be unique and no more than 100 characters.

* The description is required,  and should be in markdown format

* The user must be a member of the sysadmin administrators role. 

---

## PUT /api/Groups/{slug}

Updates an existing group with the matching `slug`. The body of the request must include the following information.

* __Slug__ - The unique external ID for the group.  _For example _YorkCodeDojo_

* __IsActive__ - Is the community currently active.

* __Name__ - The unique name for the group / community.  _For example York Code Dojo_

* __Description__ - A description of the group in markdown

* __SlackChannelName__ - The name of this groups slack channel

* __LogoInBase64__ - The group's logo.

### Business Rules

* The slug is required,  cannot contain spaces, must be unique and no more than 100 characters.  

* The name is required, must be unique and no more than 100 characters. (NEEDS TEST)

* The description is required,  and should be in markdown format

* The group must exist  (NEEDS TEST)

* The user must be a member of the sysadmin administrators role.  (NEEDS TEST)

* If the Slug or Name changes then the events must also be updated (NEEDS TEST)

---

## DELETE /api/Groups/{groupslug}

Allows a group to be deleted if it has no events.

### Business Rules

* The user must have the System Administrator role

* The group can not have any events

* If the group is successfully deleted then 204 is returned.

* If the group does not exist then 204 is returned.
  