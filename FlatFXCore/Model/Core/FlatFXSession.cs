using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatFXCore.BussinessLayer;
using System.Threading;
using System.Globalization;
using FlatFXCore.Model.User;

namespace FlatFXCore.Model.Core
{
    /// <summary>
    /// Defines the site session.
    /// </summary>
    /// <remarks>
    /// Used to cache only the needed data for the current user!
    /// </remarks>
    public class FlatFXSession
    {
        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the user role.
        /// </summary>
        public Consts.UserRoles UserRole { get; set; }
        /// <summary>
        /// Initializes a new instance of the SiteSession class.
        /// </summary>
        /// <param name="db">The data context.</param>
        /// <param name="user">The current user.</param>
        public FlatFXSession(ApplicationDBContext db, ApplicationUser user)
        {
            this.UserID = user.Id;
            this.UserName = user.UserName;
            this.UserRole = user.UserRole;
        }
    }
}
