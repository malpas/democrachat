using System;
using Democrachat.Db;

namespace Democrachat.Chat
{
    public class MuteService
    {
        private IUserService _userService;

        public MuteService(IUserService userService)
        {
            _userService = userService;
        }
        
        public MuteRequestResult TryAddMuteTime(string username, int silver, int callerId)
        {
            var callerData = _userService.GetDataById(callerId);
            var targetData = _userService.GetDataByUsername(username);
            if (callerData == null || targetData == null)
            {
                return MuteRequestResult.INVALID_USERNAME;
            }
            if (callerData.Silver < silver)
            {
                return MuteRequestResult.NOT_ENOUGH_SILVER;
            }
            _userService.AddMuteTime(targetData.Id, TimeSpan.FromSeconds(silver * 5));
            _userService.SubtractSilver(callerId, silver);
            return MuteRequestResult.OK;
        }

        public enum MuteRequestResult
        {
            OK,
            INVALID_USERNAME,
            NOT_ENOUGH_SILVER
        }
    }
}