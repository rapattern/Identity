using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity.InMemory
{
    public class InMemoryUserStore<TUser> : 
        IUserStore<TUser, string>, 
        IUserLoginStore<TUser, string>, 
        IUserRoleStore<TUser, string>,
        IUserClaimStore<TUser, string>, 
        IUserPasswordStore<TUser, string>, 
        IUserSecurityStampStore<TUser, string>,
        IUserEmailStore<TUser, string>,
        IUserLockoutStore<TUser, string>,
        IUserPhoneNumberStore<TUser, string>
        where TUser : InMemoryUser
    {
        private readonly Dictionary<UserLoginInfo, TUser> _logins =
            new Dictionary<UserLoginInfo, TUser>(new LoginComparer());

        private readonly Dictionary<string, TUser> _users = new Dictionary<string, TUser>();

        public IQueryable<TUser> Users
        {
            get { return _users.Values.AsQueryable(); }
        }

        public Task<IList<Claim>> GetClaims(TUser user)
        {
            return Task.FromResult(user.Claims);
        }

        public Task AddClaim(TUser user, Claim claim)
        {
            user.Claims.Add(claim);
            return Task.FromResult(0);
        }

        public Task RemoveClaim(TUser user, Claim claim)
        {
            user.Claims.Remove(claim);
            return Task.FromResult(0);
        }

        public Task AddLogin(TUser user, UserLoginInfo login)
        {
            user.Logins.Add(login);
            _logins[login] = user;
            return Task.FromResult(0);
        }

        public Task RemoveLogin(TUser user, UserLoginInfo login)
        {
            var logs =
                user.Logins.Where(l => l.ProviderKey == login.ProviderKey && l.LoginProvider == login.LoginProvider)
                .ToList();
            foreach (var l in logs)
            {
                user.Logins.Remove(l);
                _logins[l] = null;
            }
            return Task.FromResult(0);
        }

        public Task<IList<UserLoginInfo>> GetLogins(TUser user)
        {
            return Task.FromResult(user.Logins);
        }

        public Task<TUser> Find(UserLoginInfo login)
        {
            if (_logins.ContainsKey(login))
            {
                return Task.FromResult(_logins[login]);
            }
            return Task.FromResult<TUser>(null);
        }

        public Task SetPasswordHash(TUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHash(TUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPassword(TUser user)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task AddToRole(TUser user, string role)
        {
            user.Roles.Add(role);
            return Task.FromResult(0);
        }

        public Task RemoveFromRole(TUser user, string role)
        {
            user.Roles.Remove(role);
            return Task.FromResult(0);
        }

        public Task<IList<string>> GetRoles(TUser user)
        {
            return Task.FromResult(user.Roles);
        }

        public Task<bool> IsInRole(TUser user, string role)
        {
            return Task.FromResult(user.Roles.Contains(role));
        }

        public Task SetSecurityStamp(TUser user, string stamp)
        {
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStamp(TUser user)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        public Task Create(TUser user)
        {
            _users[user.Id] = user;
            return Task.FromResult(0);
        }

        public Task Update(TUser user)
        {
            _users[user.Id] = user;
            return Task.FromResult(0);
        }

        public Task<TUser> FindById(string userId)
        {
            if (_users.ContainsKey(userId))
            {
                return Task.FromResult(_users[userId]);
            }
            return Task.FromResult<TUser>(null);
        }

        public void Dispose()
        {
        }

        public Task<TUser> FindByName(string userName)
        {
            return Task.FromResult(Users.FirstOrDefault(u => String.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase)));
        }

        public Task Delete(TUser user)
        {
            if (user == null || !_users.ContainsKey(user.Id))
            {
                throw new InvalidOperationException("Unknown user");
            }
            _users.Remove(user.Id);
            return Task.FromResult(0);
        }

        public Task SetEmail(TUser user, string email)
        {
            user.Email = email;
            return Task.FromResult(0);
        }

        public Task<string> GetEmail(TUser user)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmed(TUser user)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmed(TUser user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task<TUser> FindByEmail(string email)
        {
            return Task.FromResult(Users.FirstOrDefault(u => String.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)));
        }

        public Task<DateTimeOffset> GetLockoutEndDate(TUser user)
        {
            return Task.FromResult(user.LockoutEnd);
        }

        public Task SetLockoutEndDate(TUser user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEnd = lockoutEnd;
            return Task.FromResult(0);
        }

        public Task<int> IncrementAccessFailedCount(TUser user)
        {
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCount(TUser user)
        {
            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCount(TUser user)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabled(TUser user)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabled(TUser user, bool enabled)
        {
            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task SetPhoneNumber(TUser user, string phoneNumber)
        {
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumber(TUser user)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmed(TUser user)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmed(TUser user, bool confirmed)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        private class LoginComparer : IEqualityComparer<UserLoginInfo>
        {
            public bool Equals(UserLoginInfo x, UserLoginInfo y)
            {
                return x.LoginProvider == y.LoginProvider && x.ProviderKey == y.ProviderKey;
            }

            public int GetHashCode(UserLoginInfo obj)
            {
                return (obj.ProviderKey + "--" + obj.LoginProvider).GetHashCode();
            }
        }
    }
}