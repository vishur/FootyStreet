﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Framework.Data.Properties {
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
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Framework.Data.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Business Logic error.  Audit Entity is missing required properties; {0} is using {1} as it&apos;s Audit Entity, and by definition an Audit Entity must have the following properties. {{ public string AuditUserName {{ get; set; }} public DateTime AuditDate {{ get; set; }} }}To Fix : Goto &quot;{2}&quot; and add a Properties..
        /// </summary>
        internal static string AuditEntityIsMissingPropertiesErrorMessage {
            get {
                return ResourceManager.GetString("AuditEntityIsMissingPropertiesErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A previous unit of work was not closed.  This unit of work was created at the following call stack: {0}..
        /// </summary>
        internal static string BeginUnitOfWorkException {
            get {
                return ResourceManager.GetString("BeginUnitOfWorkException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Completed processing Business Logic for assembly: {0}..
        /// </summary>
        internal static string CompletedBusinessLogic {
            get {
                return ResourceManager.GetString("CompletedBusinessLogic", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Business Logic error.  ICreate{6}&lt;&gt; does not have the correct {6}EntityType. {0} : ICreate{6}&lt;{1}&gt; does not use the type TEntity  as &quot;{2}&quot;. To Fix : Goto &quot;{3}&quot; and change the generic of ICreate{6} to &quot;{4}&quot; or remove DBSet&lt;{2}&gt; from DbContext &quot;{5}&quot; to use &quot;{1}&quot; as your {6} entity.&quot;.
        /// </summary>
        internal static string CreatorHasDifferentGenericErrorMessage {
            get {
                return ResourceManager.GetString("CreatorHasDifferentGenericErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Business Logic error.  DbSet is not in DbContext; &quot;{0}&quot; : ICreate{5}&lt;{1}&gt;uses DbContext &quot;{2}&quot;. This context does not have {1} defined. To Fix : Goto &quot;{3}&quot; and add a Property of type DbSet&lt;{4}&gt;..
        /// </summary>
        internal static string CreatorsGenericNotInDbContextErrorMessage {
            get {
                return ResourceManager.GetString("CreatorsGenericNotInDbContextErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Business Logic error.  {0} has the properties BeginDate and EndDate, in order for the logic surrounding date ranges to work, you must specify a [RangeUniqueKey] on the parent relationship..
        /// </summary>
        internal static string DateRangeMissingKey {
            get {
                return ResourceManager.GetString("DateRangeMissingKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Business Logic error.  Entity is the primary entity of multiple repositories. {0} is used by both {1} and {2}.  You can only have one repository per entity..
        /// </summary>
        internal static string EntityIsUsedByMultipleRepositoriesErrorMessage {
            get {
                return ResourceManager.GetString("EntityIsUsedByMultipleRepositoriesErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Business Logic error.  DbSet is not in DbContext. &quot;{0}&quot; : EntityRepository&lt;TDbContext, TEntity, ...&gt; has TDbContext of &quot;{1}&quot; which does not have a TEntity &quot;{2}&quot;. To Fix : Goto &quot;{3}&quot; and add a Property of type DbSet&lt;{2}&gt;..
        /// </summary>
        internal static string EntityNotInDbContextErrorMessage {
            get {
                return ResourceManager.GetString("EntityNotInDbContextErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Business Logic error.  Unable to map properties from entity {0} to audit {1}.  Property {2} was not mapped properly. Check name, case, type, read, and write of the properties, or implement ICreateAudit&lt;{1}&gt; on {0}..
        /// </summary>
        internal static string InvalidPropertyMappingErrorMessage {
            get {
                return ResourceManager.GetString("InvalidPropertyMappingErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to        IEntityRepository&lt;{0}&gt; was mapped to DefaultRepository&lt;{1}, {0}&gt;.
        /// </summary>
        internal static string MappedToDefaultRepository {
            get {
                return ResourceManager.GetString("MappedToDefaultRepository", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to         IEntityRepository&lt;{0}&gt; was mapped to {1}.
        /// </summary>
        internal static string MappedToRepository {
            get {
                return ResourceManager.GetString("MappedToRepository", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BusinessLogic error.  Entity type &quot;{0}&quot; is defined in two or more DbSets in &quot;{1}&quot; : DbContext. To Fix : Goto &quot;{2}&quot; and make sure there is only one Property of type DbSet&lt;{0}&gt;..
        /// </summary>
        internal static string MultipleDbSetErrorMessage {
            get {
                return ResourceManager.GetString("MultipleDbSetErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BusinessLogic error.  Namespace(s) in BusinessLogic Attribute are not found: &quot;{0}&quot;. To Fix : Locate the [assembly: BusinessLogic(...)] attribute in the properties, and validate the namespaces..
        /// </summary>
        internal static string NamespaceNotFoundErrorMessage {
            get {
                return ResourceManager.GetString("NamespaceNotFoundErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Audit Entity {0} a constructor that takes 0 arguments..
        /// </summary>
        internal static string NoDefaultConstructorForAuditErrorMessage {
            get {
                return ResourceManager.GetString("NoDefaultConstructorForAuditErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please call RepositoryRegistrar.Register(unityContainer) as part of your initialization. This class, located in Framework.Data.Repository, registers a mapping from IUnitOfWorkFactory to UnitOfWorkFactory and from IObjectSourceManagerFactory to ObjectSourceManagerFactory.
        ///
        /// If you want a different mapping than this default, you may register your own mappings of IUnitOfWorkFactory and IObjectSourceManagerFactory to your own implementations..
        /// </summary>
        internal static string PleaseCallRepositoryRegistrarRegisterUnity {
            get {
                return ResourceManager.GetString("PleaseCallRepositoryRegistrarRegisterUnity", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A {0} cannot have have its ranges overlap.  The end date of a previous date must be at least one day before the begin of the next range..
        /// </summary>
        internal static string RangeCannotOverlap {
            get {
                return ResourceManager.GetString("RangeCannotOverlap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A {0} cannot have have an end date for its last range element..
        /// </summary>
        internal static string RangeEndDateNotNull {
            get {
                return ResourceManager.GetString("RangeEndDateNotNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A {0} cannot have have an end date null for any date except its last..
        /// </summary>
        internal static string RangeEndDateNull {
            get {
                return ResourceManager.GetString("RangeEndDateNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A {0} must be a continuous range.  Each end date must be one day off from the next begin date..
        /// </summary>
        internal static string RangeMustBeAContinuous {
            get {
                return ResourceManager.GetString("RangeMustBeAContinuous", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ReferenceTableDatabaseName.
        /// </summary>
        internal static string ReferenceTableDatabaseName {
            get {
                return ResourceManager.GetString("ReferenceTableDatabaseName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to framework.data.referencetables.
        /// </summary>
        internal static string ReferenceTablesSectionName {
            get {
                return ResourceManager.GetString("ReferenceTablesSectionName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You must register a mapping from IUnitOfWork to an implementation of IUnitOfWork.  You can use one of the provided implementations or your own.  The implementations provided are Framework.Data.Repository.EF.DesktopUnitWork (appropriate for unit testing for example) and Framework.Data.WebIntegration.WebUnitOfWork (appropriate for use in ASP.NET applications)..
        /// </summary>
        internal static string RegisterIUnitOfWorkMapping {
            get {
                return ResourceManager.GetString("RegisterIUnitOfWorkMapping", resourceCulture);
            }
        }
    }
}
