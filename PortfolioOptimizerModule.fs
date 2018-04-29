module PortfolioOptimizerModule
open SQLExecutionModule
open IBApi
open EWrapperImplementModule
open MLOptimizationModule
open System.Configuration
open System.Data
let toDateTime(expir:string) = 
    new System.DateTime(int(expir.Substring(0,4)),int(expir.Substring(4,2)),int(expir.Substring(6,2)))
type PortfolioOptimizer(wrapper:EWrapperImplement,client:EClientSocket,SQL:SQLExecution,MLOptimizer:MLOptimization) =
    let collectDMS(instr) = 
        let (default_entries,stk, dms,predMu,predSd) = MLOptimizer.computeForStock(instr)
        (default_entries,stk,dms,predMu,predSd)
    let mutable combosNumerical = []
    let mutable sortedCombosNumerical = []
    let OptimizationRoutine(dmsSet:List<int*string*(List<string*float>*List<string*float>*float*float)*float*float>,instrumentMineList:List<DataRow>) = 
        let mutable selectedCombo = []
        let corrDatConcat = [for (default_entries,stk,dms,predMu,predSd) in dmsSet do yield [for (default_entries2,stk2,dms2,predMu2,predSd2) in dmsSet do yield (stk,dms,predMu,predSd,stk2,dms2,predMu2,predSd2,MLOptimizer.STK_Correlation(dms,dms2)) ] ] |> List.concat
        let portfolioSize = int(ConfigurationManager.AppSettings.["OptimizationPortfolio"])
        let allStockCombos = MLOptimizer.Combinations([for dms in dmsSet do yield (dms |> fun (i,s,dm,f1,f2) -> s)],portfolioSize)
        let mutable ctr = 0
        if true then //for stockComboPartition in partitionedStockCombos do
            if true then //let t = System.Threading.Thread(fun () -> 
                for stockComboSet in allStockCombos do // stockComboPartition do
                    ctr <- ctr + 1
                    let mutable innerNumericalMu = [for stk in stockComboSet do yield ((corrDatConcat |> List.find(fun (s1,dms1,predMu1,predSd1,s9,dms9,predMu9,predSd9,corr) -> stk = s1) |> fun (sa1,dmsa1,predMua1,predSda1,sa2,dmsa2,predMua2,predSda2,corra) -> System.Math.Abs(predMua1)))]
                    let mutable innerNumericalVar = [for stk in stockComboSet do yield ((corrDatConcat |> List.find(fun (s1,dms1,predMu1,predSd1,s2,dms2,predMu2,predSd2,corr) -> stk = s1) |> fun (sa1,dmsa1,predMua1,predSda1,sa2,dmsa2,predMua2,predSda2,corra) -> (predMua1,dmsa1) |> fun (pmua1,(la,lb,mu,sd)) -> sd / float(innerNumericalMu.Length) ) ) ] 
                    let mutable innerStkBuySell = [for stk in stockComboSet do yield ((corrDatConcat |> List.find(fun (s1,dms1,predMu1,predSd1,s2,dms2,predMu2,predSd2,corr) -> stk = s1) |> fun (sa1,dmsa1,predMua1,predSda1,sa2,dmsa2,predMua2,predSda2,corra) -> (stk,(predMua1 > 0.)) ) ) ]
                    for stk in stockComboSet do
                        for stk2 in stockComboSet do
                            if stk <> stk2 then
                                innerNumericalVar <- (corrDatConcat |> List.find(fun (s1,dms1,predMu1,predSd1,s2,dms2,predMu2,predSd2,corr) -> stk = s1 && stk2 = s2) |> fun (sa1,dmsa1,predMua1,predSda1,sa2,dmsa2,predMua2,predSda2,corra) -> (predMua1,predMua2,dmsa1,dmsa2,corra) |> fun (pmua1,pmua2,(la,lb,mu1,sd1),(l1a,l1b,mua1,sda1),corrb) -> (if pmua1 < 0. then -1. else 1.) * (if pmua2 < 0. then -1. else 1.) * sd1*sda1*corrb/(float(innerNumericalMu.Length)**2.))::innerNumericalVar 
                    combosNumerical <- (innerStkBuySell,innerNumericalMu |> List.average, innerNumericalVar |> List.sum)::combosNumerical
        sortedCombosNumerical <- combosNumerical |> List.sortBy(fun (stkLst,muPort,varPort) -> (muPort/varPort))
        selectedCombo <- sortedCombosNumerical.[0] |> fun (lst,mu,var) -> lst
        [ for (stk,bs) in selectedCombo do yield (stk,bs,(instrumentMineList |> List.find(fun instr -> instr.["Symbol"].ToString() = stk)) ) ]
    member this.OptimizedCollection(dmsSet:List<int*string*(List<string*float>*List<string*float>*float*float)*float*float>,instrumentMineList:List<DataRow>) = 
        OptimizationRoutine(dmsSet,instrumentMineList)
    member this.OptimizedAllocations(optimization:List<int*string*(List<string*float>*List<string*float>*float*float)*float*float*DataRow>) = 
        let stockComboSet = [for (default_entries,stk,dms,predMu,predSd,dr) in optimization do yield stk ]
        let OptimizationSize = int(ConfigurationManager.AppSettings.["OptimizationSize"].ToString())
        let corrDatConcat = [for (default_entries,stk,dms,predMu,predSd,dr) in optimization do yield [for (default_entries2,stk2,dms2,predMu2,predSd2,dr2) in optimization do yield (stk,dms,predMu,predSd,dr,stk2,dms2,predMu2,predSd2,dr2,MLOptimizer.STK_Correlation(dms,dms2)) ] ] |> List.concat
        let mutable allocate = [for (default_entries,stk,dms,predMu,predSd,dr) in optimization do yield (stk,1) ]
        let mutable innerNumericalMu = [for stk in stockComboSet do yield ((corrDatConcat |> List.find(fun (s1,dms1,predMu1,predSd1,dra1,s9,dms9,predMu9,predSd9,drx2,corr) -> stk = s1) |> fun (sa1,dmsa1,predMua1,predSda1,drx1,sa2,dmsa2,predMua2,predSda2,dr2,corra) -> float(snd (allocate |> List.find(fun (st,am) -> st = sa1) )) * System.Math.Abs(predMua1)))]
        let mutable innerNumericalVar = [for stk in stockComboSet do yield ((corrDatConcat |> List.find(fun (s1,dms1,predMu1,predSd1,dra1,s2,dms2,predMu2,predSd2,drx2,corr) -> stk = s1) |> fun (sa1,dmsa1,predMua1,predSda1,drx1,sa2,dmsa2,predMua2,predSda2,dr2,corra) -> (sa1,predMua1,dmsa1) |> fun (sta1,pmua1,(la,lb,mu,sd)) -> float(snd (allocate |> List.find(fun (st,am) -> st = sta1) ) ) * sd / float(innerNumericalMu.Length) ) ) ] 
        for stk in stockComboSet do
            for stk2 in stockComboSet do
                if stk <> stk2 then
                    innerNumericalVar <- (corrDatConcat |> List.find(fun (s1,dms1,predMu1,predSd1,drx,s2,dms2,predMu2,predSd2,drx2,corr) -> stk = s1 && stk2 = s2) |> fun (sa1,dmsa1,predMua1,predSda1,dry,sa2,dmsa2,predMua2,predSda2,dry2,corra) -> (sa1,predMua1,sa2,predMua2,dmsa1,dmsa2,corra) |> fun (sta1,pmua1,sta2,pmua2,(la,lb,mu1,sd1),(l1a,l1b,mua1,sda1),corrb) -> float(snd (allocate |> List.find(fun (st,am) -> st = sa1) ) ) * (if pmua1 < 0. then -1. else 1.) * float(snd (allocate |> List.find(fun (st,am) -> st = sa1) ) ) * (if pmua2 < 0. then -1. else 1.) * sd1*sda1*corrb/(float(innerNumericalMu.Length)**2.))::innerNumericalVar 
        let flatMu = (innerNumericalMu |> List.sum) / float(allocate |> List.collect(fun (a,b) -> [b]) |> List.sum)
        let flatVar = (innerNumericalVar |> List.sum) / float(allocate |> List.collect(fun (a,b) -> [b]) |> List.sum)
        let allocations = OptimizationSize - (allocate |> List.collect(fun (a,b) -> [b]) |> List.sum)
        let allocationDifference(allocation:List<string*int>) =
            if allocation.Length > 1 then
                let allo = (allocation |> List.collect(fun (a,b) -> [b]) |> List.sortBy(fun x -> abs x))
                let diff = allo.[allo.Length - 1] - allo.[0]
                let pct = float(diff) / float(allo |> List.sum)
                pct <= 0.3
            else
                true
        let rec allocation(stockToAllocate, remainingAllocations, allocated,currMu,currVar,stockAllocated) = 
            if remainingAllocations = 0 then
                (allocated,currMu,currVar,stockAllocated)
            else
                let mutable newAllocate = 
                    if stockToAllocate = "NON_STOCK" then
                        allocated
                    else
                        (stockToAllocate, 1 + (snd (allocated |> List.find(fun (c,d) -> c = stockToAllocate))))::[for (a,b) in (allocated |> List.where(fun (x,y) -> x <> stockToAllocate)) do yield (a,b)]
                innerNumericalMu <- [for stk in stockComboSet do yield ((corrDatConcat |> List.find(fun (s1,dms1,predMu1,predSd1,dra,s9,dms9,predMu9,predSd9,drb,corr) -> stk = s1) |> fun (sa1,dmsa1,predMua1,predSda1,dra1,sa2,dmsa2,predMua2,predSda2,drb1,corra) -> float(snd (newAllocate |> List.find(fun (st,am) -> st = sa1) )) * System.Math.Abs(predMua1)))]
                innerNumericalVar <- [for stk in stockComboSet do yield ((corrDatConcat |> List.find(fun (s1,dms1,predMu1,predSd1,dra,s2,dms2,predMu2,predSd2,drb,corr) -> stk = s1) |> fun (sa1,dmsa1,predMua1,predSda1,dra1,sa2,dmsa2,predMua2,predSda2,drb1,corra) -> (sa1,predMua1,dmsa1) |> fun (sta1,pmua1,(la,lb,mu,sd)) -> float(snd (newAllocate |> List.find(fun (st,am) -> st = sta1) ) ) * sd / float(innerNumericalMu.Length) ) ) ] 
                for stk in stockComboSet do
                    for stk2 in stockComboSet do
                        if stk <> stk2 then
                            innerNumericalVar <- (corrDatConcat |> List.find(fun (s1,dms1,predMu1,dre,predSd1,s2,dms2,predMu2,predSd2,dre1,corr) -> stk = s1 && stk2 = s2) |> fun (sa1,dmsa1,predMua1,predSda1,drc,sa2,dmsa2,predMua2,predSda2,drd,corra) -> (sa1,predMua1,sa2,predMua2,dmsa1,dmsa2,corra) |> fun (sta1,pmua1,sta2,pmua2,(la,lb,mu1,sd1),(l1a,l1b,mua1,sda1),corrb) -> float(snd (newAllocate |> List.find(fun (st,am) -> st = sta1) ) ) * (if pmua1 < 0. then -1. else 1.) * float(snd (newAllocate |> List.find(fun (st,am) -> st = sta2) ) ) * (if pmua2 < 0. then -1. else 1.) * sd1*sda1*corrb/(float(innerNumericalMu.Length)**2.))::innerNumericalVar 
                let allocMu = (innerNumericalMu |> List.sum) / float(newAllocate |> List.collect(fun (a,b) -> [b]) |> List.sum)
                let allocVar = (innerNumericalVar |> List.sum) / float(newAllocate |> List.collect(fun (a,b) -> [b]) |> List.sum)
                if remainingAllocations >= 1  then 
                    if allocationDifference(newAllocate) && (allocMu >= currMu || allocVar <= currVar) then
                        let mutable (pcAllocate,pcMu,pcVar,pcStk) = allocation(stockToAllocate,remainingAllocations - 1,newAllocate,allocMu,allocVar,stockToAllocate) 
                        for stk in stockComboSet do
                            let (futAllocate,futMu,futVar,futStk) = allocation(stk,(remainingAllocations - 1),newAllocate,allocMu,allocVar,stockToAllocate)
                            if allocationDifference(futAllocate) && (pcMu <= futMu || pcVar >= futVar) then 
                                pcMu <- futMu
                                pcVar <- futVar
                                pcStk <- futStk
                                pcAllocate <- futAllocate
                        (pcAllocate,pcMu,pcVar,pcStk)
                    else
                        let mutable (pcMu,pcVar,pcAllocate,pcStk,pcoStk) = (allocMu,allocVar,newAllocate,stockToAllocate,stockToAllocate) 
                        for stk in stockComboSet do
                            let (futAllocate,futMu,futVar,futStk) = allocation(stk,(remainingAllocations - 1),newAllocate,allocMu,allocVar,stockToAllocate)
                            if allocationDifference(futAllocate) && (pcMu <= futMu || pcVar >= futVar) then 
                                pcMu <- futMu
                                pcVar <- futVar
                                pcStk <- futStk
                                pcAllocate <- futAllocate
                                pcoStk <- stk
                        (pcAllocate,pcMu,pcVar,pcStk)
                else
                    (newAllocate,allocMu,allocVar,stockToAllocate)
        let (wgAllocated,wgMu,wgVar,wgStk) = allocation(stockComboSet.[0],allocations,allocate,flatMu,flatVar,"NON_STOCK")
        [for (stocka,allocate) in wgAllocated do yield (stocka,allocate,(optimization |> List.find(fun (default_entries,stk,dms,predMu,predSd,dr) -> stk = stocka))) ]