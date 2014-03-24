using Framework.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes",
            Justification = "Static member is designed to represent information about generic.")]
    public abstract partial class EntityRepository<TDbContext, TPrimaryEntity>
    {
        #region Static Fields

        /// <summary>
        /// The AuditEntity Type that is created from coping the Primary Entity
        /// </summary>
        private static Type auditEntityType;

        /// <summary>
        /// If the Primary Entity implements IAuditTracker
        /// </summary>
        private static bool isAuditTracker;

        /// <summary>
        /// Determines how Audit is performed
        /// </summary>
        private static bool isCreateAudit;

        /// <summary>
        /// Determines if type is Date Range
        /// </summary>
        private static bool isDateRange;

        /// <summary>
        /// If the Primary Entity implements IInsertTracker and not IAuditTracker
        /// </summary>
        private static bool isInsertTracker;

        /// <summary>
        /// If the Primary Entity implements IUpdateTracker and not IAuditTracker
        /// </summary>
        private static bool isUpdateTracker;

        /// <summary>
        /// If the Primary Entity implements isKeyEntityTracker and not IAuditTracker
        /// </summary>
        private static bool isKeyEntityTracker;


        /// <summary>
        /// Determines if the static fields have been loaded or not.
        /// </summary>
        private static bool isStaticFieldsLoaded = false;

        /// <summary>
        /// Locks the Load method
        /// </summary>
        private static readonly object staticFieldLoadLock = new object();

        #endregion Static Fields

        #region Public and Protected Properties
        /// <summary>
        /// The AuditEntity Type that is created from coping the Primary Entity
        /// </summary>
        public static Type AuditEntityType
        {
            get
            {
                if (!isStaticFieldsLoaded)
                {
                    Load();
                }
                return auditEntityType;
            }
        }

        /// <summary>
        /// Determines how Audit is performed
        /// </summary>
        public static bool IsCreateAudit
        {
            get
            {
                if (!isStaticFieldsLoaded)
                {
                    Load();
                }
                return isCreateAudit;
            }
        }

        /// <summary>
        /// Determines is Date Range
        /// </summary>
        public static bool IsDateRange
        {
            get
            {
                if (!isStaticFieldsLoaded)
                {
                    Load();
                }
                return isDateRange;
            }
        }

        /// <summary>
        /// If the Primary Entity implements IAuditTracker
        /// </summary>
        public static bool IsAuditTracker
        {
            get
            {
                if (!isStaticFieldsLoaded)
                {
                    Load();
                }
                return isAuditTracker;
            }
        }

        /// <summary>
        /// If the Primary Entity implements IInsertTracker and not IAuditTracker
        /// </summary>
        public static bool IsInsertTracker
        {
            get
            {
                if (!isStaticFieldsLoaded)
                {
                    Load();
                }
                return isInsertTracker;
            }
        }


        /// <summary>
        /// If the Primary Entity implements IUpdateTracker and not IAuditTracker
        /// </summary>
        public static bool IsUpdateTracker
        {
            get
            {
                if (!isStaticFieldsLoaded)
                {
                    Load();
                }
                return isUpdateTracker;
            }
        }


        /// <summary>
        /// If the Primary Entity implements IUpdateTracker and not IAuditTracker
        /// </summary>
        public static bool IsKeyEntityTracker
        {
            get
            {
                if (!isStaticFieldsLoaded)
                {
                    Load();
                }
                return isKeyEntityTracker;
            }
        }
        #endregion

        #region Private Members

        /// <summary>
        /// Loads Static information that defines how the Repository works for generic arguments.
        /// </summary>
        /// <remarks>
        /// Static information will be generic based,
        /// so they will have different values based on the generics.
        ///
        /// By checking these in the static constructor, it makes the
        /// methods faster since the your doing a boolean check instead of
        /// an 'is' check.
        ///
        /// Be careful when editing since it is a static constructor,
        /// an error in here will cause the type to not to work.
        /// </remarks>
        private static void Load()
        {
            if (!isStaticFieldsLoaded)
            {
                lock (staticFieldLoadLock)
                {
                    if (!isStaticFieldsLoaded)
                    {
                        isStaticFieldsLoaded = true;
                        Type primaryType = typeof(TPrimaryEntity);
                        Type[] interfaces = primaryType.GetInterfaces();
                        isAuditTracker = interfaces.Contains(typeof(IAuditTracker));
                        if (!isAuditTracker) // if the primary entity is an audit tracker,

                        // we don't want to override the audit information created by the live entity.
                        {
                            foreach (Type primaryInterface in interfaces)
                            {
                                if (primaryInterface == typeof(IInsertTracker))
                                {
                                    isInsertTracker = true;
                                }
                                else if (primaryInterface == typeof(IUpdateTracker))
                                {
                                    isUpdateTracker = true;
                                }
                                else if (primaryInterface == typeof(IKeyEntity))
                                {
                                    isKeyEntityTracker = true;
                                }
                                else if (primaryInterface == typeof(IDateRange))
                                {
                                    isDateRange = true;
                                }
                                else if (AuditEntityType == null && primaryInterface.IsGenericType &&
                                    primaryInterface.GetGenericTypeDefinition() == typeof(ICreateAudit<>))
                                {
                                    auditEntityType = primaryInterface.GetGenericArguments().Single();
                                    isCreateAudit = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion Private Members

    }
}
