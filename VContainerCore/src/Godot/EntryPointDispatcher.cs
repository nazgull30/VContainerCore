// using System;
// using System.Collections.Generic;
// using Godot;
// using VContainer.Internal;

// namespace VContainer.Godot;

// public class EntryPointDispatcher
// {
//     private readonly IObjectResolver _container;

//     public EntryPointDispatcher(IObjectResolver container)
//     {
//         _container = container;
//     }

//     public void Dispatch()
//     {
//         EntryPointExceptionHandler exceptionHandler = null;
//         try
//         {
//             exceptionHandler = _container.Resolve<EntryPointExceptionHandler>();
//         }
//         catch (VContainerException ex) when (ex.InvalidType == typeof(EntryPointExceptionHandler))
//         {
//         }

//         var initializables = _container.Resolve<ContainerLocal<IReadOnlyList<IInitializable>>>().Value;
//         for (var i = 0; i < initializables.Count; i++)
//         {
//             try
//             {
//                 initializables[i].Initialize();
//             }
//             catch (Exception ex)
//             {
//                 if (exceptionHandler != null)
//                     exceptionHandler.Publish(ex);
//                 else
//                     GD.PrintErr(ex);
//             }
//         }
//     }
// }
