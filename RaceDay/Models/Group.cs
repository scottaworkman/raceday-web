//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RaceDay.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Group
    {
        public Group()
        {
            this.Events = new HashSet<Event>();
            this.GroupMembers = new HashSet<GroupMember>();
        }
    
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string FacebookId { get; set; }
        public string ApiKey { get; set; }
    
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<GroupMember> GroupMembers { get; set; }
    }
}
