TODO!

Add tests around call from slack.  Maybe mock using PACT

Add approvedBy,  approvedOn,  rejectedBy,  rejectedOn to the User model.

Can the user login if they have been rejected?  Is that the same as banned?


Frontend
--------
Improve the registration details screen.
Allow the user to view the attendees of an event.
Create button test data
Email support
Slack integration 


System adminstrators can :  Ban Users (same as reject?)
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

Notify members when things happen

Groups are sometimes retrieved twice.  Could cache.   

Rename controllers
