using OpenNos.Domain.I18N;

namespace OpenNos.Data.Interfaces
{
    public interface II18NDto
    {
        string Key { get; set; }
        RegionType RegionType { get; set; }
        string Text { get; set; }
    }
}
