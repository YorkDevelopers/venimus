
TODO!

System adminstrators can :  Ban Users
### POST /api/BannedUsers/{userslug}
### DELETE /api/BannedUsers/{userslug}

Users join groups,   which means they can view events and sign up.
Group admins can then approve users,  this means they can now see the list of group members / event attendees

Make someone a group admin

Some person details are only visible to group administrators

Group Administrators
View attendees - messages + diatary  requirements

Logic around banned users

Logging (driven by events?)

Automatically call /user/connected from Auth0?   Or should /user/connected indicate if this is a new user?

Include user's profile picture.

Allow an email address to be changed.  What should happen?  Confirm via email?  Update member details in groups+events

Localise error messages

Base64 images  as per Kevin's suggestion

Add Blob storage

Denormalise Groups into the user record

Notify members when things happen

dotnet test --filter "VenimusAPIs.Tests.DeleteEvent"

Groups are sometimes retrieved twice.  Could cache.   

Rename controllers