using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Democrachat.Auth
{
    public class RegisterSpamCheckService
    {
        private List<Entry> _entries = new();
        
        public bool CheckIp(IPAddress? ip)
        {
            ip ??= IPAddress.None;
            ClearOldEntries();
            if (_entries.Any(entry => entry.ip.Equals(ip)))
            {
                return false;
            }
            _entries.Add(new Entry(ip, DateTime.Now));
            return true;
        }

        private void ClearOldEntries()
        {
            _entries = _entries
                .Where(entry => DateTime.Now < entry.time + TimeSpan.FromMinutes(15))
                .ToList();
        }

        record Entry(IPAddress ip, DateTime time);
    }
}