
TODO!

System adminstrators can :  Ban Users
### POST /api/BannedUsers/{userslug}
### DELETE /api/BannedUsers/{userslug}

Group administrators can : List (including people) / Create / Edit / Delete Events / View Event Members
### GET /api/Groups/{GroupSlug}/Members
### GET /api/Groups/{GroupSlug}/Events/{EventSlug}/Members

### PUT /api/Groups/{GroupSlug}/Events
### DELETE /api/Groups/{GroupSlug}/Events

Rename GroupName or Slug needed to update events

Logic around banned users

Logging (driven by events?)

Automatically call /user/connected from Auth0?   Or should /user/connected indicate if this is a new user?

Include user's profile picture.

Allow an email address to be changed.  What should happen?  Confirm via email?

Localise error messages