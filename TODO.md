
TODO!

System adminstrators can :  Ban Users
### POST /api/BannedUsers/{userslug}
### DELETE /api/BannedUsers/{userslug}


Logic around banned users

Logging (driven by events?)

Automatically call /user/connected from Auth0?   Or should /user/connected indicate if this is a new user?

Include user's profile picture.

Allow an email address to be changed.  What should happen?  Confirm via email?

Localise error messages

Add events to process actions,  such as updating events after a group slug/name changes.

Base64 images  as per Kevin's suggestion

Add Blob storage

Rename Group.Member.ID 

Denormalise Group.Member

Add IsAttending to GET /api/groups/{GroupSlug}/Events/{EventSlug}/Members

Notify members when things happen

Improve permissions model.


dotnet test --filter "VenimusAPIs.Tests.DeleteEvent"


        [CallerMustBeGroupAdministrator]  -> returns 404 if the group does not exist.   returns 403 if not an admin
        [CallerMustBelongToGroup] -> returns 404 if the group does not exist.   returns 403 if not a member
