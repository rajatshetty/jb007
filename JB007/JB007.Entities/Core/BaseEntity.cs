using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JB007.Entities.Core
{
    public abstract class BaseEntity
    {
        private Nullable<DateTime> _CreatedDate;
        private Nullable<DateTime> _ModifiedDate;
        public long Id { get; set; }
        public Guid TenantId { get; set; }
        public DateTime CreatedDate
        {
            get
            {
                if (_CreatedDate != null)
                    return _CreatedDate.Value.Year == 1 ? DateTime.Now : _CreatedDate.Value;
                else
                    return DateTime.Now;
            }
            set
            {
                _CreatedDate = value;
            }
        }
        public DateTime ModifiedDate
        {
            get
            {
                if (_ModifiedDate != null)
                    return _ModifiedDate.Value.Year == 1 ? DateTime.Now : _ModifiedDate.Value;
                else
                    return DateTime.Now;
            }
            set
            {
                _ModifiedDate = value;
            }
        }
        public bool Status { get; set; }
    }
}
