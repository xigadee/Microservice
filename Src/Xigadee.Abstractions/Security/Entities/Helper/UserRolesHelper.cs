using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This helper class is used to manage the role addition and removal.
    /// </summary>
    public static class UserRolesHelper
    {
        /// <summary>
        /// Determines whether the user has that role.
        /// </summary>
        /// <param name="ur">The user role object.</param>
        /// <param name="role">The role.</param>
        /// <returns>
        ///   <c>true</c> if the specified role has role; otherwise, <c>false</c>.
        /// </returns>
        public static bool RoleExists(this UserRoles ur, string role) =>
            ur.Roles?.Contains(RolePrepare(role)) ?? false;

        /// <summary>
        /// Adds the roles.
        /// </summary>
        /// <param name="ur">The user role object.</param>
        /// <param name="role">The role.</param>
        public static void RoleAdd(this UserRoles ur, string role)
        {
            role = RolePrepare(role);

            if (string.IsNullOrEmpty(role))
                return;

            if (ur.RoleExists(role))
                return;

            if (ur.Roles == null)
                ur.Roles = new List<string>();

            ur.Roles.Add(role);
        }

        private static string RolePrepare(string role) => (role ?? "").Trim().ToLowerInvariant();

        /// <summary>
        /// Adds the roles.
        /// </summary>
        /// <param name="ur">The user role object.</param>
        /// <param name="role">The role.</param>
        public static bool RoleDelete(this UserRoles ur, string role)
        {
            role = RolePrepare(role);

            if (string.IsNullOrEmpty(role))
                return false;

            if (!ur.RoleExists(role))
                return false;

            if (ur.Roles == null)
                return false;

            return ur.Roles.Remove(role);
        }
    }
}
