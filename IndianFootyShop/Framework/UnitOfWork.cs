using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    /// <summary>
    /// Represents a Unit of Work.
    /// Any entity that is created during this UnitOfWork from
    /// the EntityRepository (Factory), will share the disposal event of
    /// the UnitOfWork
    /// </summary>
    public sealed class UnitOfWork : IDisposable
    {
        /// <summary>
        /// We are wrapping the Disposable
        /// to have a simiplier API.
        /// </summary>
        internal Disposable<UnitOfWork> Disposable;

        /// <summary>
        /// The event of saving.
        /// </summary>
        internal event EventHandler SaveChanges;

        /// <summary>
        /// The point where the unit of work began.
        /// </summary>
        internal StackTrace StartingPoint { get; private set; }

        /// <summary>
        /// If true, the EntityRepository (Factory)
        /// will save any changes when the unit of
        /// work is completed automatically.
        /// <para>Otherwise you specifically
        /// will have to save.</para>
        /// </summary>
        public bool SaveOnCompletion { get; set; }

        /// <summary>
        /// Internal contructor
        /// </summary>
        /// <param name="saveOnCompletion"></param>
        internal UnitOfWork(bool saveOnCompletion = false)
        {
            StartingPoint = new StackTrace(2);
            SaveOnCompletion = saveOnCompletion;
            Disposable = new Disposable<UnitOfWork>(this);

            //All the logic is within the generic Disposable
        }

        /// <summary>
        /// Has the UnitOfWork already be disposed of.
        /// </summary>
        public bool HasDisposed { get { return Disposable.HasDisposed; } }

        /// <summary>
        /// Saves any changes that has happened in the unit of work
        /// </summary>
        public void Save()
        {
            if (HasDisposed)
            {
                throw new ObjectDisposedException("Unit of Work has been disposed of.");
            }
            if (SaveChanges != null)
            {
                SaveChanges(this, new EventArgs());
            }
        }

        /// <summary>
        /// Disposes the UnitOfWork along
        /// with Repositories created under this UnitOfWork.
        /// </summary>
        public void Dispose()
        {
            Disposable.Dispose();
        }
    }
}
