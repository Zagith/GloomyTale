using GloomyTale.DAL.EF;
using GloomyTale.Data;

namespace GloomyTale.Mapper.Mappers
{
    public class LogDropMapper
    {
        #region Methods

        public static bool ToLogDrop(LogDropDTO input, LogDrop output)
        {
            if (input == null)
            {
                return false;
            }

            output.LogId = input.LogId;
            output.CharacterId = input.CharacterId;
            output.ItemVNum = input.ItemVNum;
            output.ItemName = input.ItemName;
            output.Amount = input.Amount;
            output.Map = input.Map;
            output.X = input.X;
            output.Y = input.Y;
            output.IpAddress = input.IpAddress;
            output.Timestamp = input.Timestamp;

            return true;
        }

        public static bool ToLogDropDTO(LogDrop input, LogDropDTO output)
        {
            if (input == null)
            {
                return false;
            }

            output.LogId = input.LogId;
            output.CharacterId = input.CharacterId;
            output.ItemVNum = input.ItemVNum;
            output.ItemName = input.ItemName;
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
