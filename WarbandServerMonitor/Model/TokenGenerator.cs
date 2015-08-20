using System;
using System.Collections.Concurrent;

namespace WarbandServerMonitor.Model
{
    public class TokenGenerator
    {
        // .NET Framework does not have ConcurrentHashSet so use ConcurrentDictionary with meaningless value parameter.
        private readonly ConcurrentDictionary<Int16, byte> _usedTokens = new ConcurrentDictionary<short, byte>();
        private readonly Random _rnd = new Random();

        public Int16 Get()
        {
            Int16 newToken;
            do
            {
                newToken = (Int16)((_rnd.Next(0, 16) << 12) + (_rnd.Next(0, 16) << 8) + (_rnd.Next(0, 16) << 4));
            } while (newToken == 0 || !_usedTokens.TryAdd(newToken, 0));
            return newToken;
        }

        public void Free(Int16 token)
        {
            byte _;
            _usedTokens.TryRemove(token, out _);
        }
    }
}
