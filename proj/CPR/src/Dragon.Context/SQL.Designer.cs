﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Dragon.Context {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SQL {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SQL() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Dragon.Context.SQL", typeof(SQL).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM {TABLE}
        ///           WHERE [ParentID] = @ParentID AND [ChildID] = @ChildID.
        /// </summary>
        internal static string SqlPermissionStore_DeletePermissionNode {
            get {
                return ResourceManager.GetString("SqlPermissionStore_DeletePermissionNode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM {TABLE}
        ///           WHERE [LID] = @LID.
        /// </summary>
        internal static string SqlPermissionStore_DeletePermissionRight {
            get {
                return ResourceManager.GetString("SqlPermissionStore_DeletePermissionRight", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM  {TABLE}.
        /// </summary>
        internal static string SqlPermissionStore_GetAllPermissionNodes {
            get {
                return ResourceManager.GetString("SqlPermissionStore_GetAllPermissionNodes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM  {TABLE}.
        /// </summary>
        internal static string SqlPermissionStore_GetAllPermissionRights {
            get {
                return ResourceManager.GetString("SqlPermissionStore_GetAllPermissionRights", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM  {TABLE}
        ///WHERE [ParentID] = @ParentID.
        /// </summary>
        internal static string SqlPermissionStore_GetPermissionNodesByParentID {
            get {
                return ResourceManager.GetString("SqlPermissionStore_GetPermissionNodesByParentID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM  {TABLE}
        ///WHERE [ChildID] = @ChildID.
        /// </summary>
        internal static string SqlPermissionStore_GetPermissionParentNodes {
            get {
                return ResourceManager.GetString("SqlPermissionStore_GetPermissionParentNodes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM  {TABLE} WHERE NodeID=@NodeID.
        /// </summary>
        internal static string SqlPermissionStore_GetPermissionRightsByNode {
            get {
                return ResourceManager.GetString("SqlPermissionStore_GetPermissionRightsByNode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO  {TABLE}
        ///           ([LID]
        ///           ,[ParentID]
        ///           ,[ChildID])
        ///     VALUES
        ///           (NEWID()
        ///           ,@ParentID
        ///           ,@ChildID).
        /// </summary>
        internal static string SqlPermissionStore_InsertPermissionNode {
            get {
                return ResourceManager.GetString("SqlPermissionStore_InsertPermissionNode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO  {TABLE}
        ///           ([LID]
        ///           ,[NodeID] 
        ///           ,[SubjectID]
        ///           ,[Spec]
        ///           ,[Inherit])
        ///     VALUES
        ///           (@LID
        ///           ,@NodeID
        ///           ,@SubjectID
        ///           ,@Spec
        ///           ,@Inherit).
        /// </summary>
        internal static string SqlPermissionStore_InsertPermissionRight {
            get {
                return ResourceManager.GetString("SqlPermissionStore_InsertPermissionRight", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM  {TABLE} WHERE UserID = @UserID AND [Key] = @Key.
        /// </summary>
        internal static string SqlProfileStore_Get {
            get {
                return ResourceManager.GetString("SqlProfileStore_Get", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO  {TABLE} ([LID],[UserID], [Key], [Value]) VALUES (@LID, @UserID, @Key, @Value).
        /// </summary>
        internal static string SqlProfileStore_Insert {
            get {
                return ResourceManager.GetString("SqlProfileStore_Insert", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE {TABLE} SET [Value] = @Value WHERE UserID = @UserID AND [Key] = @Key.
        /// </summary>
        internal static string SqlProfileStore_Update {
            get {
                return ResourceManager.GetString("SqlProfileStore_Update", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM  {TABLE} WHERE [SessionID] =@SessionID.
        /// </summary>
        internal static string SqlSessionStore_Delete {
            get {
                return ResourceManager.GetString("SqlSessionStore_Delete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM  {TABLE}
        ///WHERE [SessionID] =@SessionID.
        /// </summary>
        internal static string SqlSessionStore_Get {
            get {
                return ResourceManager.GetString("SqlSessionStore_Get", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO  {TABLE} ([SessionID],[Hash],[Location],[UserID],[Expires])
        ///VALUES			   (@SessionID,@Hash,@Location,@UserID, @Expires).
        /// </summary>
        internal static string SqlSessionStore_Insert {
            get {
                return ResourceManager.GetString("SqlSessionStore_Insert", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE  {TABLE} SET 
        ///[Hash]= @Hash,
        ///[Expires]	= @Expires,
        ///[Location]	= @Location,
        ///[UserID]	= @UserID
        ///WHERE [SessionID] = @SessionID.
        /// </summary>
        internal static string SqlSessionStore_Update {
            get {
                return ResourceManager.GetString("SqlSessionStore_Update", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT [RegistrationID]
        ///      ,[UserID]
        ///      ,[Service]
        ///      ,[Key]
        ///      ,[Secret]
        ///FROM {TABLE}
        ///WHERE [Service] = @Service AND [Key] = @Key.
        /// </summary>
        internal static string SqlUserStore_GetByServiceAndKey {
            get {
                return ResourceManager.GetString("SqlUserStore_GetByServiceAndKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT [RegistrationID]
        ///      ,[UserID]
        ///      ,[Service]
        ///      ,[Key]
        ///      ,[Secret]
        ///FROM {TABLE}
        ///WHERE [UserID] = @UserID.
        /// </summary>
        internal static string SqlUserStore_GetByUserID {
            get {
                return ResourceManager.GetString("SqlUserStore_GetByUserID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO {TABLE}
        ///           ([RegistrationID]
        ///           ,[UserID]
        ///           ,[Service]
        ///           ,[Key]
        ///           ,[Secret])
        ///     VALUES
        ///           (@RegistrationID
        ///           ,@UserID
        ///           ,@Service
        ///           ,@Key
        ///           ,@Secret).
        /// </summary>
        internal static string SqlUserStore_Insert {
            get {
                return ResourceManager.GetString("SqlUserStore_Insert", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE {TABLE}
        ///SET [UserID]  = @UserID
        ///   ,[Service] = @Service
        ///   ,[Key]     = @Key
        ///   ,[Secret]  = @Secret
        ///WHERE RegistrationID = @RegistrationID.
        /// </summary>
        internal static string SqlUserStore_Update {
            get {
                return ResourceManager.GetString("SqlUserStore_Update", resourceCulture);
            }
        }
    }
}
