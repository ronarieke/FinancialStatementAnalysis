namespace FundamentalDataAnalysis
open System.Data
open System.Xml
open System.Collections
open FundamentalDataExtraction
type FinStatements(dataRow:DataRow) = 
    let mutable report = ReportsObject(dataRow)
    let mutable finStatements = report.finStatement
    let cashFlowAnaylsis(buySell) = 
        try 
            if buySell then
            
                true
            else    
                true
        with ex -> false
    let balanceSheetAnalysis(buySell) = 
        try
            if buySell then 
                // check TotalCurrentAssets
                let avgAtca = ([for (tup,lst) in finStatements.BalanceSheet do yield (snd ([for (hey,pey) in lst do yield (hey,pey)] |> List.where(fun (heyy,peyy) -> heyy = "ATCA")).[0])] |> List.average)
                let sdAtca = ([for (tup,lst) in finStatements.BalanceSheet do yield (snd ([for (hey,pey) in lst do yield (hey,(pey - avgAtca)**2.) ] |> List.where(fun(hey,pey) -> hey = "ATCA")).[0])] |> List.average) |> fun var -> (var ** (1./2.))
                let currAtca = (snd ([for ((tupa,tupb,tupc),lst) in finStatements.BalanceSheet do yield (System.Convert.ToDateTime(tupb),  (snd ([for (hey,pey) in lst do yield (hey,pey)] |> List.where(fun (hey,pey) -> hey = "ATCA")).[0]))] |> List.sortBy(fun (daa,pey) -> (System.DateTime.Now - daa).TotalDays)).[0])
                    // check Total Current Liabilities
                let avgLtcl = ([for (tup,lst) in finStatements.BalanceSheet do yield (snd ([for (hey,pey) in lst do yield (hey,pey)] |> List.where(fun (heyy,peyy) -> heyy = "LTCL")).[0])] |> List.average)
                let sdLtcl = ([for (tup,lst) in finStatements.BalanceSheet do yield (snd ([for (hey,pey) in lst do yield (hey,(pey - avgLtcl)**2.) ] |> List.where(fun(hey,pey) -> hey = "LTCL")).[0])] |> List.average) |> fun var -> (var ** (1./2.))
                let currLtcl = (snd ([for ((tupa,tupb,tupc),lst) in finStatements.BalanceSheet do yield (System.Convert.ToDateTime(tupb),  (snd ([for (hey,pey) in lst do yield (hey,pey)] |> List.where(fun (hey,pey) -> hey = "LTCL")).[0]))] |> List.sortBy(fun (daa,pey) -> (System.DateTime.Now - daa).TotalDays)).[0])
                        // check Goodwill
                let avgAgwi = ([for (tup,lst) in finStatements.BalanceSheet do yield (snd ([for (hey,pey) in lst do yield (hey,pey)] |> List.where(fun (heyy,peyy) -> heyy = "AGWI")).[0])] |> List.average)
                let sdAgwi = ([for (tup,lst) in finStatements.BalanceSheet do yield (snd ([for (hey,pey) in lst do yield (hey,(pey - avgAgwi)**2.) ] |> List.where(fun(hey,pey) -> hey = "AGWI")).[0])] |> List.average) |> fun var -> (var ** (1./2.))
                let currAgwi = (snd ([for ((tupa,tupb,tupc),lst) in finStatements.BalanceSheet do yield (System.Convert.ToDateTime(tupb),  (snd ([for (hey,pey) in lst do yield (hey,pey)] |> List.where(fun (hey,pey) -> hey = "AGWI")).[0]))] |> List.sortBy(fun (daa,pey) -> (System.DateTime.Now - daa).TotalDays)).[0])

                ((currAtca > avgAtca - sdAtca) && ((currLtcl < avgLtcl + sdLtcl) || (currAgwi > avgAgwi - sdAgwi))) || ((currLtcl < avgLtcl + sdLtcl) && ((currAtca > avgAtca - sdAtca) || (currAgwi > avgAgwi - sdAgwi))) || ((currAgwi > avgAgwi - sdAgwi) && ((currLtcl < avgLtcl + sdLtcl) || (currAtca > avgAtca - sdAtca)))
            else 
                // check TotalCurrentAssets
                let avgAtca = ([for (tup,lst) in finStatements.BalanceSheet do yield (snd ([for (hey,pey) in lst do yield (hey,pey)] |> List.where(fun (heyy,peyy) -> heyy = "ATCA")).[0])] |> List.average)
                let sdAtca = ([for (tup,lst) in finStatements.BalanceSheet do yield (snd ([for (hey,pey) in lst do yield (hey,(pey - avgAtca)**2.) ] |> List.where(fun(hey,pey) -> hey = "ATCA")).[0])] |> List.average) |> fun var -> (var ** (1./2.))
                let currAtca = (snd ([for ((tupa,tupb,tupc),lst) in finStatements.BalanceSheet do yield (System.Convert.ToDateTime(tupb),  (snd ([for (hey,pey) in lst do yield (hey,pey)] |> List.where(fun (hey,pey) -> hey = "ATCA")).[0]))] |> List.sortBy(fun (daa,pey) -> (System.DateTime.Now - daa).TotalDays)).[0])
                    // check Total Current Liabilities
                let avgLtcl = ([for (tup,lst) in finStatements.BalanceSheet do yield (snd ([for (hey,pey) in lst do yield (hey,pey)] |> List.where(fun (heyy,peyy) -> heyy = "LTCL")).[0])] |> List.average)
                let sdLtcl = ([for (tup,lst) in finStatements.BalanceSheet do yield (snd ([for (hey,pey) in lst do yield (hey,(pey - avgLtcl)**2.) ] |> List.where(fun(hey,pey) -> hey = "LTCL")).[0])] |> List.average) |> fun var -> (var ** (1./2.))
                let currLtcl = (snd ([for ((tupa,tupb,tupc),lst) in finStatements.BalanceSheet do yield (System.Convert.ToDateTime(tupb),  (snd ([for (hey,pey) in lst do yield (hey,pey)] |> List.where(fun (hey,pey) -> hey = "LTCL")).[0]))] |> List.sortBy(fun (daa,pey) -> (System.DateTime.Now - daa).TotalDays)).[0])
                        // check Goodwill
                let avgAgwi = ([for (tup,lst) in finStatements.BalanceSheet do yield (snd ([for (hey,pey) in lst do yield (hey,pey)] |> List.where(fun (heyy,peyy) -> heyy = "AGWI")).[0])] |> List.average)
                let sdAgwi = ([for (tup,lst) in finStatements.BalanceSheet do yield (snd ([for (hey,pey) in lst do yield (hey,(pey - avgAgwi)**2.) ] |> List.where(fun(hey,pey) -> hey = "AGWI")).[0])] |> List.average) |> fun var -> (var ** (1./2.))
                let currAgwi = (snd ([for ((tupa,tupb,tupc),lst) in finStatements.BalanceSheet do yield (System.Convert.ToDateTime(tupb),  (snd ([for (hey,pey) in lst do yield (hey,pey)] |> List.where(fun (hey,pey) -> hey = "AGWI")).[0]))] |> List.sortBy(fun (daa,pey) -> (System.DateTime.Now - daa).TotalDays)).[0])
 
                ((currAtca < avgAtca + sdAtca) && ((currLtcl > avgLtcl - sdLtcl) || (currAgwi < avgAgwi + sdAgwi))) || ((currLtcl > avgLtcl - sdLtcl) && ((currAtca < avgAtca + sdAtca) || (currAgwi < avgAgwi + sdAgwi))) || ((currAgwi < avgAgwi + sdAgwi) && ((currLtcl > avgLtcl - sdLtcl) || (currAtca < avgAtca + sdAtca)))

        with ex -> false
    let incomeStatementAnalysis(buySell) = 
        try
            if buySell then
                // check for revenue
                let avgRev = ( [ for (tup,lst) in finStatements.IncomeStatement do yield (snd ([ for (hey,pey) in lst do yield (hey,pey) ] |> List.where(fun (heyy,peyy) -> heyy = "SREV")).[0])] |> List.average)
                let sdRev = ([ for (tup,lst) in finStatements.IncomeStatement do yield (snd ([for (hey,pey) in lst do yield (hey, (pey - avgRev)**2.) ] |> List.where(fun (hey,pey) -> hey = "SREV")).[0])] |> List.average) |> fun var -> (var ** (1./2.))
                let curRev = (snd ([for ((tupa,tupb,tupc),lst) in finStatements.IncomeStatement do yield (System.Convert.ToDateTime(tupb), (snd ([ for (hey,pey) in lst do yield (hey,pey)] |> List.where(fun (hey,pey) -> hey = "SREV")).[0]))] |> List.sortBy(fun(daa,pey)-> (System.DateTime.Now - daa).TotalDays)).[0])
                    // check for net income 
                let avgNinc = ( [ for (tup,lst) in finStatements.IncomeStatement do yield (snd ([ for (hey,pey) in lst do yield (hey,pey) ] |> List.where(fun (heyy,peyy) -> heyy = "NINC")).[0])] |> List.average)
                let sdNinc = ([ for (tup,lst) in finStatements.IncomeStatement do yield (snd ([for (hey,pey) in lst do yield (hey, (pey - avgNinc)**2.) ] |> List.where(fun (hey,pey) -> hey = "NINC")).[0])] |> List.average) |> fun var -> (var ** (1./2.))
                let curNinc = (snd ([for ((tupa,tupb,tupc),lst) in finStatements.IncomeStatement do yield (System.Convert.ToDateTime(tupb), (snd ([ for (hey,pey) in lst do yield (hey,pey)] |> List.where(fun (hey,pey) -> hey = "NINC")).[0]))] |> List.sortBy(fun(daa,pey)-> (System.DateTime.Now - daa).TotalDays)).[0])
                        // check for Total Operating Expense
                let avgEtoe = ( [ for (tup,lst) in finStatements.IncomeStatement do yield (snd ([ for (hey,pey) in lst do yield (hey,pey) ] |> List.where(fun (heyy,peyy) -> heyy = "ETOE")).[0])] |> List.average)
                let sdEtoe = ([ for (tup,lst) in finStatements.IncomeStatement do yield (snd ([for (hey,pey) in lst do yield (hey, (pey - avgEtoe)**2.) ] |> List.where(fun (hey,pey) -> hey = "ETOE")).[0])] |> List.average) |> fun var -> (var ** (1./2.))
                let curEtoe = (snd ([for ((tupa,tupb,tupc),lst) in finStatements.IncomeStatement do yield (System.Convert.ToDateTime(tupb), (snd ([ for (hey,pey) in lst do yield (hey,pey)] |> List.where(fun (hey,pey) -> hey = "ETOE")).[0]))] |> List.sortBy(fun(daa,pey)-> (System.DateTime.Now - daa).TotalDays)).[0])
                ((curNinc > avgNinc - sdNinc) && ((curRev > avgRev - sdRev) || (curEtoe < avgEtoe + sdEtoe))) || ((curRev > avgRev - sdRev) && ((curEtoe < avgEtoe + sdEtoe) || (curNinc > avgNinc - sdNinc))) || ((curEtoe <  avgEtoe + sdEtoe) && ((curRev > avgRev - sdRev) || (curNinc > avgNinc - sdNinc)))
            else
                // check for revenue
                let avgRev = ( [ for (tup,lst) in finStatements.IncomeStatement do yield (snd ([ for (hey,pey) in lst do yield (hey,pey) ] |> List.where(fun (heyy,peyy) -> heyy = "SREV")).[0])] |> List.average)
                let sdRev = ([ for (tup,lst) in finStatements.IncomeStatement do yield (snd ([for (hey,pey) in lst do yield (hey, (pey - avgRev)**2.) ] |> List.where(fun (hey,pey) -> hey = "SREV")).[0])] |> List.average) |> fun var -> (var ** (1./2.))
                let curRev = (snd ([for ((tupa,tupb,tupc),lst) in finStatements.IncomeStatement do yield (System.Convert.ToDateTime(tupb), (snd ([ for (hey,pey) in lst do yield (hey,pey)] |> List.where(fun (hey,pey) -> hey = "SREV")).[0]))] |> List.sortBy(fun(daa,pey)-> (System.DateTime.Now - daa).TotalDays)).[0])
                let avgNinc = ( [ for (tup,lst) in finStatements.IncomeStatement do yield (snd ([ for (hey,pey) in lst do yield (hey,pey) ] |> List.where(fun (heyy,peyy) -> heyy = "NINC")).[0])] |> List.average)
                let sdNinc = ([ for (tup,lst) in finStatements.IncomeStatement do yield (snd ([for (hey,pey) in lst do yield (hey, (pey - avgNinc)**2.) ] |> List.where(fun (hey,pey) -> hey = "NINC")).[0])] |> List.average) |> fun var -> (var ** (1./2.))
                let curNinc = (snd ([for ((tupa,tupb,tupc),lst) in finStatements.IncomeStatement do yield (System.Convert.ToDateTime(tupb), (snd ([ for (hey,pey) in lst do yield (hey,pey)] |> List.where(fun (hey,pey) -> hey = "NINC")).[0]))] |> List.sortBy(fun(daa,pey)-> (System.DateTime.Now - daa).TotalDays)).[0])
                
                let avgEtoe = ( [ for (tup,lst) in finStatements.IncomeStatement do yield (snd ([ for (hey,pey) in lst do yield (hey,pey) ] |> List.where(fun (heyy,peyy) -> heyy = "ETOE")).[0])] |> List.average)
                let sdEtoe = ([ for (tup,lst) in finStatements.IncomeStatement do yield (snd ([for (hey,pey) in lst do yield (hey, (pey - avgEtoe)**2.) ] |> List.where(fun (hey,pey) -> hey = "ETOE")).[0])] |> List.average) |> fun var -> (var ** (1./2.))
                let curEtoe = (snd ([for ((tupa,tupb,tupc),lst) in finStatements.IncomeStatement do yield (System.Convert.ToDateTime(tupb), (snd ([ for (hey,pey) in lst do yield (hey,pey)] |> List.where(fun (hey,pey) -> hey = "ETOE")).[0]))] |> List.sortBy(fun(daa,pey)-> (System.DateTime.Now - daa).TotalDays)).[0])
                ((curNinc < avgNinc + sdNinc) && ((curRev < avgRev + sdRev) || (curEtoe > avgEtoe - sdEtoe))) || ((curRev < avgRev + sdRev) && ((curEtoe > avgEtoe - sdEtoe) || (curNinc < avgNinc + sdNinc))) || ((curEtoe >  avgEtoe - sdEtoe) && ((curRev < avgRev + sdRev) || (curNinc < avgNinc + sdNinc)))
        with ex -> false
    member this.finStatementsAnalysis(buySell) = 
        cashFlowAnaylsis(buySell) && balanceSheetAnalysis(buySell) && incomeStatementAnalysis(buySell)
    member this.ReportObject = report
    member this.X = "F#"

