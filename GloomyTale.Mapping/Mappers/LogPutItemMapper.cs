using GloomyTale.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public class LogPutItemMapper
    {
        #region Methods

        public static bool ToLogPutItem(LogPutItemDTO input, LogPutItem output)
        {
            if (input == null)
            {
                return false;
            }

            output.LogId = input.LogId;
            output.CharacterId = input.CharacterId;
            output.ItemVNum = input.ItemVNum;
            output.Amount = input.Amount;
            output.Map = input.Map;
            output.X = input.X;
            output.Y = input.Y;
            output.IpAddress = input.IpAddress;
            output.Timestamp = input.Timestamp;

            return true;
        }

        public static bool ToLogPutItemDTO(LogPutItem input, LogPutItemDTO output)
        {
            if (input == null)
            {
                return false;
            }

            output.LogId = input.LogId;
            output.CharacterId = input.CharacterId;
            output.ItemVNum = input.ItemVNum;
            output.Amount = input.Amount;
            output.Map = input.Map;
            output.X = input.X;
            output.Y = input.Y;
            output.IpAddress = input.IpAddress;
            output.Timestamp = input.Timestamp;

            return true;
        }

        #endregion
    }
}
