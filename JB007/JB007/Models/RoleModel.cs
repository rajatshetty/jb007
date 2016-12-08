using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JB007.Models
{
    public class RoleModel
    {
        #region Properties
        public Int64 Id { get; set; }
        public Guid TenantId { get; set; }
        public string ERoleCode { get; set; }
        public string SelectedModules { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool Status { get; set; }
        #endregion

        #region Constructor
        public RoleModel()
        {

        }
        #endregion
    }
}