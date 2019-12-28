using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VenimusAPIs.Models;

namespace VenimusAPIs.Tests.Infrastucture
{
    public class Data
    {
        private readonly Random _random;

        public Data(int seed = -1)
        {
            _random = seed switch
            {
                -1 => new Random(),
                _ => new Random(seed)
            };
        }

        internal string CreateStringLongerThan(int requiredLength)
        {
            var result = string.Empty;

            while (result.Length < requiredLength)
            {
                result += Guid.NewGuid().ToString();
            }

            return result;
        }

        public Models.GroupEvent CreateEvent(Models.Group group, Action<Models.GroupEvent> modifier = null)
        {
            var newEvent = Create<Models.GroupEvent>(evt =>
            {
                evt.GroupId = group.Id;
                evt.GroupSlug = group.Slug;
                evt.GroupName = group.Name;
                evt.StartTimeUTC = DateTime.UtcNow.AddDays(1);
                evt.EndTimeUTC = DateTime.UtcNow.AddDays(2);
                evt.MaximumNumberOfAttendees = 100;
                evt.GuestsAllowed = true;
                evt.FoodProvided = true;
                evt.Members = new List<GroupEventAttendee>();
                evt.Questions = new List<Question>
                {
                    new Question { Caption = "Caption1", Code = "Q1", QuestionType = QuestionType.Text },
                    new Question { Caption = "Caption2", Code = "Q2", QuestionType = QuestionType.Text },
                };
            });

            if (modifier != null)
            {
                modifier.Invoke(newEvent);
            }

            return newEvent;
        }

        public T Create<T>(Action<T> modifier = null)
        {
            var properties = typeof(T).GetProperties();
            var obj = (T)Activator.CreateInstance(typeof(T));

            foreach (var property in properties)
            {
                if (property.GetSetMethod() == null)
                {
                    continue;
                }

                var propertyType = property.PropertyType.Name;

                if (propertyType == "Nullable`1")
                {
                    propertyType = property.PropertyType.GenericTypeArguments[0].Name;
                }

                if (propertyType == typeof(string).Name)
                {
                    var customAttributes = property.GetCustomAttributes(true);
                    var customAttribute =
                        customAttributes.SingleOrDefault(aa => aa.GetType() == typeof(MaxLengthAttribute));
                    if (customAttribute != null)
                    {
                        var maxLength = ((MaxLengthAttribute)customAttribute).Length;
                        var value = Guid.NewGuid().ToString();
                        if (value.Length > maxLength)
                        {
                            value = value.Substring(0, maxLength);
                        }

                        property.SetValue(obj, value);
                    }
                    else
                    {
                        property.SetValue(obj, Guid.NewGuid().ToString());
                    }
                }
                else if (propertyType == typeof(DateTime).Name)
                {
                    var date = DateTime.Parse("2019-03-15").AddSeconds(_random.Next(100000));
                    property.SetValue(obj, date);
                }
                else if (propertyType == typeof(bool).Name)
                {
                    property.SetValue(obj, CreateBool());
                }
                else if (propertyType == typeof(Guid).Name)
                {
                    property.SetValue(obj, Guid.NewGuid());
                }
                else if (propertyType == typeof(double).Name)
                {
                    property.SetValue(obj, _random.NextDouble());
                }
                else if (propertyType == typeof(int).Name)
                {
                    var customAttributes = property.GetCustomAttributes(true);
                    var customAttribute =
                        customAttributes.SingleOrDefault(aa => aa.GetType() == typeof(RangeAttribute));
                    if (customAttribute != null)
                    {
                        var min = (int)((RangeAttribute)customAttribute).Minimum;
                        var max = (int)((RangeAttribute)customAttribute).Maximum;

                        property.SetValue(obj, _random.Next(min, max));
                    }
                    else
                    {
                        property.SetValue(obj, _random.Next(int.MaxValue));
                    }
                }
                else if (propertyType == typeof(short).Name)
                {
                    property.SetValue(obj, (short)_random.Next(short.MaxValue));
                }
                else if (propertyType == typeof(decimal).Name)
                {
                    var customAttributes = property.GetCustomAttributes(true);
                    var customAttribute =
                        customAttributes.SingleOrDefault(aa => aa.GetType() == typeof(RangeAttribute));
                    if (customAttribute != null)
                    {
                        var min = (int)((RangeAttribute)customAttribute).Minimum;
                        var max = (int)((RangeAttribute)customAttribute).Maximum;

                        property.SetValue(obj, _random.Next(min, max) / 100.00M);
                    }
                    else
                    {
                        property.SetValue(obj, (decimal)_random.NextDouble());
                    }
                }
                else if (propertyType == typeof(byte[]).Name)
                {
                    var b = new byte[_random.Next(1000) + 1];
                    _random.NextBytes(b);
                    property.SetValue(obj, b);
                }
            }

            if (modifier != null)
            {
                modifier.Invoke(obj);
            }

            return obj;
        }

        internal GroupEventAttendee AddEventHost(GroupEvent theEvent, User user)
        {
            if (theEvent.Members == null)
            {
                theEvent.Members = new System.Collections.Generic.List<GroupEventAttendee>();
            }

            var attendee = new GroupEventAttendee
            {
                UserId = user.Id,
                Host = true,
                Bio = user.Bio,
                DisplayName = user.DisplayName,
                EmailAddress = user.EmailAddress,
                Fullname = user.Fullname,
                Pronoun = user.Pronoun,
            };

            theEvent.Members.Add(attendee);

            return attendee;
        }

        internal GroupEventAttendee AddEventSpeaker(GroupEvent theEvent, User user)
        {
            if (theEvent.Members == null)
            {
                theEvent.Members = new System.Collections.Generic.List<GroupEventAttendee>();
            }

            var attendee = new GroupEventAttendee
            {
                UserId = user.Id,
                Speaker = true,
                Bio = user.Bio,
                DisplayName = user.DisplayName,
                EmailAddress = user.EmailAddress,
                Fullname = user.Fullname,
                Pronoun = user.Pronoun,
            };

            theEvent.Members.Add(attendee);

            return attendee;
        }

        internal GroupEventAttendee AddEventAttendee(GroupEvent theEvent, User user, int numberOfGuests = 0)
        {
            if (theEvent.Members == null)
            {
                theEvent.Members = new System.Collections.Generic.List<GroupEventAttendee>();
            }

            var newAttendee = new GroupEventAttendee
            {
                UserId = user.Id,
                NumberOfGuests = numberOfGuests,
                SignedUp = true,
                DietaryRequirements = "No milk",
                Bio = user.Bio,
                DisplayName = user.DisplayName,
                EmailAddress = user.EmailAddress,
                Fullname = user.Fullname,
                Pronoun = user.Pronoun,
            };

            theEvent.Members.Add(newAttendee);

            return newAttendee;
        }

        internal GroupEventAttendee AddEventNonAttendee(GroupEvent theEvent, User user, int numberOfGuests = 0)
        {
            if (theEvent.Members == null)
            {
                theEvent.Members = new System.Collections.Generic.List<GroupEventAttendee>();
            }

            var newAttendee = new GroupEventAttendee
            {
                UserId = user.Id,
                NumberOfGuests = numberOfGuests,
                SignedUp = false,
                DietaryRequirements = Guid.NewGuid().ToString(),
                Bio = user.Bio,
                DisplayName = user.DisplayName,
                EmailAddress = user.EmailAddress,
                Fullname = user.Fullname,
                Pronoun = user.Pronoun,
            };

            theEvent.Members.Add(newAttendee);

            return newAttendee;
        }

        internal GroupMember AddGroupMember(Group group, User user)
        {
            if (group.Members == null)
            {
                group.Members = new List<GroupMember>();
            }

            var newMember = new GroupMember
            {
                UserId = user.Id,
                IsAdministrator = false,
                Bio = user.Bio,
                DisplayName = user.DisplayName,
                EmailAddress = user.EmailAddress,
                Fullname = user.Fullname,
                Pronoun = user.Pronoun,
                IsUserApproved = user.IsApproved,
            };

            group.Members.Add(newMember);

            return newMember;
        }

        internal void AddGroupAdministrator(Group group, User user)
        {
            if (group.Members == null)
            {
                group.Members = new List<GroupMember>();
            }

            group.Members.Add(new GroupMember
            {
                UserId = user.Id,
                IsAdministrator = true,
                Bio = user.Bio,
                DisplayName = user.DisplayName,
                EmailAddress = user.EmailAddress,
                Fullname = user.Fullname,
                Pronoun = user.Pronoun,
                IsUserApproved = true,
            });
        }

        internal string CreateString(int requiredLength) => CreateStringLongerThan(requiredLength).Substring(0, requiredLength);

        internal bool CreateBool() => _random.Next(2) == 1;

        internal int CreateInt(int minValue = 0, int maxValue = int.MaxValue) => _random.Next(minValue, maxValue);
    }
}
