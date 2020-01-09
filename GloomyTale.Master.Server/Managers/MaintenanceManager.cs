using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.Master.Managers
{
    public class MaintenanceManager
    {
        private bool _isInMaintenance;

        public void SetMaintenanceMode(bool value)
        {
            _isInMaintenance = value;
        }

        public bool GetMaintenanceMode() => _isInMaintenance;
    }
}
