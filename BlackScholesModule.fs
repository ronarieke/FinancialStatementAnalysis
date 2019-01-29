module BlackScholesModule
open System
let accuracy = 0.0001
let granularity = 0.1
let divider = 1./7.
let integral(fxn:float->float, lb, ub) = 
    let rec inneIntegral(gran,ilb,iub) = 
        if gran > accuracy then
            [ for a in [ilb..gran*divider..iub] do yield inneIntegral(gran*divider,a,(1.+a)*gran*divider)] |> List.sum
        else
            gran*divider*(fxn(ilb)+fxn(iub))/2.
    [for a in [lb..granularity..ub] do yield inneIntegral(granularity*divider,a,(1.+a)*granularity)] |> List.sum

let innePhi(x) = 
        Math.E**(-(x**2.)/2.)

let PhiIntegralTail = 
    integral(innePhi,-10.,-1.)

let Phi(x) =     
    (1./sqrt(2.*Math.PI))*integral(innePhi,-1.,x)
let phi(x) = 
    innePhi(x)/sqrt(2.*Math.PI)
let d1(S,K,r,q,sigma,tau) = 
    (Math.Log(S/K) + (r - q + (sigma**2.)/2.) * tau) / (sigma * sqrt(tau))
let d2(D1, sigma, tau) = 
    D1 - sigma*sqrt(tau)

let deltaC(S,q,tau,_PhiD1) = 
    (Math.E**(-(q*tau))) * _PhiD1
let deltaP(q,tau,_PhiND1) = 
    (Math.E**(-q*tau))*_PhiND1

let vega(S,q,tau,phiD1) = 
    S*(Math.E**(q*tau))*phiD1*sqrt(tau)

let thetaC(q,tau,S,phiD1,sigma,r,K,phiD2,_PhiD1) = 
    ((-Math.E**(-q*tau)) * (S * phiD1 * sigma) / (2. * sqrt(tau))) - (r*K*(Math.E**(-r*tau)))*phiD2 + q*S*(Math.E**(-(q*tau)))*_PhiD1
let thetaP(q,tau,S,phiD1,sigma,r,K,_PhiND2,_PhiND1) = 
    (-Math.E**(-q*tau))*((S*phiD1*sigma)/(2.*sqrt(tau))) + r*K*(Math.E**(-r*tau))*_PhiND2 - q*S*(Math.E**(-q*tau))*_PhiND1 

let rhoC(K,tau,r,_PhiD2) = 
    K*tau*(Math.E**(-r*tau))*_PhiD2
let rhoP(K,tau,r,_PhiND2) = 
    -K*tau*(Math.E**(-r*tau))*_PhiND2

let gamma(q,tau,phiD1,sigma,r,K,phiD2,S,_PhiD1) = 
    (K*(Math.E**(-r*tau)))*((phiD2)/((S**2.)*sigma*sqrt(tau)))

let Call(_PhiD1,S,_PhiD2,K,r,tau) = 
    _PhiD1*S - _PhiD2 * K * (Math.E**(-r*tau))

let Put(K,r,tau,S,C) = 
    K * (Math.E**(-r*tau)) - S + C

    