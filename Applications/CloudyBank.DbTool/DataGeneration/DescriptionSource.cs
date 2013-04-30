using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoPoco.Engine;

namespace CloudyBank.DbTool.DataGeneration
{
    public class DescriptionSource : IDatasource<String>
    {
        int count;
        String[] descr = { "FACTURE CARTE DU SUPERMARCHE 75 PARIS 06 CARTE", 
                            "FACTURE CARTE DU 160311 RATP 75 PARIS CARTE 4974XXXXXX",
                            "RETRAIT DAB 18/02/11 23H38 038770 BNP PARIS ST GERMA",
                            "FACTURE CARTE DU 181210 MONOPRIX 75 PARIS",
                            "PRELEVEMENT ABONNEMENT IMAGINE R NUM 457385", 
                            "FACTURE CARTE DU 071010 BOUYGUES TELECO 92 NEUILLY SEI"};
        public object Next(IGenerationSession session)
        {
            return descr[count++ % descr.Length];
        }
    }
}
