namespace Xigadee
{
    /// <summary>
    /// This class holders a registered process that will be polled as part of the thread loop.
    /// </summary>
    public class TaskManagerProcessContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskManagerProcessContext"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public TaskManagerProcessContext(string name)
        {
            Name = name;
        }
        /// <summary>
        /// The process priority.
        /// </summary>
        public int Ordinal { get; set; }
        /// <summary>
        /// The execute action.
        /// </summary>
        public ITaskManagerProcess Process { get; set; }
        /// <summary>
        /// The unique read-only process name.
        /// </summary>
        public string Name { get; }
    }

    /// <summary>
    /// This class holders a registered process that will be polled as part of the thread loop.
    /// </summary>
    /// <typeparam name="C">The context type.</typeparam>
    public class TaskManagerProcessContext<C>: TaskManagerProcessContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskManagerProcessContext{C}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public TaskManagerProcessContext(string name) : base(name)
        {
            Context = default(C);
        }
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        public C Context { get; set; }
    }
}
