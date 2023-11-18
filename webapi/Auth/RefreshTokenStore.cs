using Microsoft.IdentityModel.JsonWebTokens;
using System.IdentityModel.Tokens.Jwt;

namespace webapi.Auth
{
    public class RefreshTokenStore
    {
        private readonly Dictionary<string, DateTime> _revokedTokens = new Dictionary<string, DateTime>();
        private JwtSecurityTokenHandler _jwtHandler=new JwtSecurityTokenHandler();
        private readonly object _lock = new object();
        private readonly TimeSpan _cleanupInterval=TimeSpan.FromDays(1);
        public RefreshTokenStore()
        {
            var cleanupTimer = new Timer(CleanupExpiredTokens, null, _cleanupInterval, _cleanupInterval);
        }
        public void RevokeRefreshToken(string refreshToken)
        {
            lock (_lock)
            {
                DateTime expires = _jwtHandler.ReadToken(refreshToken).ValidTo;
                _revokedTokens.Add(refreshToken, expires);
            }
        }
        public bool IsRefreshTokenRevoked(string refreshToken)
        {
            lock ( _lock)
            {
                return _revokedTokens.ContainsKey(refreshToken);
            }
        }
        private void CleanupExpiredTokens(object state)
        {
            lock (_lock)
            {
                var expiredTokens = _revokedTokens.Where(pair => pair.Value <= DateTime.Now).ToList();
                foreach (var expiredToken in expiredTokens)
                {
                    _revokedTokens.Remove(expiredToken.Key);
                }
            }
        }
    }
}
