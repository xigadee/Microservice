using System;
using Xigadee;

namespace Test.Xigadee
{
    internal interface IPopulatorConsole:IPopulator
    {
        IRepositoryAsync<Guid, MondayMorningBlues> Persistence { get; }
    }
}