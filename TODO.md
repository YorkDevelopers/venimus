
TODO!

System adminstrators can :  Ban Users
### POST /api/BannedUsers/{userslug}
### DELETE /api/BannedUsers/{userslug}

Make someone a group admin

Some person details are only visible to group administrators

Group Administrators
View attendees - messages + diatary  requirements

Logic around banned users
    Cannot make any calls

Logging (driven by events?)

Automatically call /user/connected from Auth0?   Or should /user/connected indicate if this is a new user?



Allow an email address to be changed.  What should happen?  Confirm via email?  Update member details in groups+events



Denormalise Groups into the user record

Notify members when things happen

Groups are sometimes retrieved twice.  Could cache.   

Rename controllers


## Group Images
    /api/Groups      (all groups)
    /public/Groups   (only active groups)
    /api/user/groups (groups I belong to)

Add ImageURL to the group
Include the ImageURL in these results
Add endpoint to serve up the image URL.


Include user's profile picture.
Base64 images  as per Kevin's suggestion


## Future Ideas

Move images to Blob storage