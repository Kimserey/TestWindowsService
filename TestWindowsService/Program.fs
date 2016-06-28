// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System
open System.Timers
open System.Diagnostics
open Topshelf
open Topshelf.ServiceConfigurators
open Topshelf.HostConfigurators

type TownCrier() =
    let timer = new Timer(1000.)
    do timer.Elapsed.Add(fun args -> printfn "It is %s and all is well" (DateTime.Now.ToLongTimeString()))

    member x.Start() = timer.Start()
    member x.Stop() = timer.Stop()

let runService (hostCfg: HostConfigurator) = 

    hostCfg.Service<TownCrier>(Action<_>(fun (s: ServiceConfigurator<TownCrier>) ->
        s.ConstructUsing(Func<TownCrier>(fun () -> new TownCrier()))
            .WhenStarted(Action<TownCrier>(fun s -> s.Start()))
            .WhenStopped(Action<TownCrier>(fun s -> s.Stop()))
            |> ignore )) |> ignore
    
    hostCfg.RunAsLocalSystem() |> ignore
    hostCfg.SetDescription("Sample topshelf host")
    hostCfg.SetDisplayName("Display name")
    hostCfg.SetServiceName("Service name")

[<EntryPoint>]
let main argv = 

    HostFactory.Run(Action<_>(runService))
    |> ignore

    0
