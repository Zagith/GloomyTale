using OpenNos.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public static class PortalMapper
    {
        #region Methods

        public static bool ToPortal(PortalDTO input, Portal output)
        {
            if (input == null)
            {
                return false;
            }

            output.DestinationMapId = input.DestinationMapId;
            output.DestinationX = input.DestinationX;
            output.DestinationY = input.DestinationY;
            output.IsDisabled = input.IsDisabled;
            output.PortalId = input.PortalId;
            output.SourceMapId = input.SourceMapId;
            output.SourceX = input.SourceX;
            output.SourceY = input.SourceY;
            output.Type = input.Type;
            output.Side = input.Side;
            output.RequiredItem = input.RequiredItem;
            output.NomeOggetto = input.NomeOggetto;

            return true;
        }

        public static bool ToPortalDTO(Portal input, PortalDTO output)
        {
            if (input == null)
            {
                return false;
            }

            output.DestinationMapId = input.DestinationMapId;
            output.DestinationX = input.DestinationX;
            output.DestinationY = input.DestinationY;
            output.IsDisabled = input.IsDisabled;
            output.PortalId = input.PortalId;
            output.SourceMapId = input.SourceMapId;
            output.SourceX = input.SourceX;
            output.SourceY = input.SourceY;
            output.Type = input.Type;
            output.Side = input.Side;
            output.RequiredItem = input.RequiredItem;
            output.NomeOggetto = input.NomeOggetto;

            return true;
        }

        #endregion
    }
}