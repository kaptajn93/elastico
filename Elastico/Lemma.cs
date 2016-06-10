using Elastico.Areas.HelpPage.ModelDescriptions;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace Elastico
{
    
    public class Lemma
    {
        public string LemmaLanguage { get; set; }

        public string LemmaId { get; set; }

        public string LemmaOrtography { get; set; }

        public LemmaPos LemmaPos { get; set; }

        public LemmaGender LemmaGender { get; set; }

        public LemmaInflection LemmaInflection { get; set; }

        public LemmaVariants LemmaVariants { get; set; }

        public string LemmaMeAsFirst { get; set; }
        public string LemmaMeAsLast { get; set; }

        public IList<LemmaAccessoryData> LemmaAccessoryDatas { get; set; }

        public LemmaUsage Usage { get; set; }
        public LemmaPronuciatetionAll PronuciatetionAll { get; set; }
        public LemmaIllustration Illustration { get; set; }
        public LemmaAbbreviationFor AbbreviationFor { get; set; }

        public bool IsThereSoundFiles { get; set; }
        public int SoundfilesCount { get; set; }

        public bool IsThereIllustrations { get; set; }
        public int IllustrationsCount { get; set; }

    }

    public class LemmaPos
    {
        public string PosNameDan { get; set; }
        public string PosNameGyl { get; set; }
        public string PosNameEng { get; set; }
        public string PosNameLat { get; set; }
        public string PosShortNameDan { get; set; }
        public string PosShortNameGyl { get; set; }
        public string PosShortNameEng { get; set; }
        public string PosShortNameLat { get; set; }
    }

    public class LemmaGender
    {
        public string GenNameDan { get; set; }
        public string GenNameGyl { get; set; }
        public string GenNameEng { get; set; }
        public string GenNameLat { get; set; }
        public string GenShortNameDan { get; set; }
        public string GenShortNameGyl { get; set; }
        public string GenShortNameEng { get; set; }
        public string GenShortNameLat { get; set; }
    }

    public class LemmaVariants
    {
        public string LemmaVariantsRefPos;
        public string LemmaVariantsRefRef;
    }

    public class LemmaAccessoryData
    {
        public string CategoryDan { get; set; }
        public string CategoryEng { get; set; }
        public IList<LemmaReference> LemmaAccessDataReferencesRefs { get; set; }
    }

    public class LemmaReference
    {
        public string LemmaPos { get; set; }
        public string LemmaRef { get; set; }
    }

    public class LemmaInflection
    {
        public string CompactPresentation { get; set; }

        public IList<LemmaTablePresentation> TablePresentations { get; set; }

        [Nested]
        public IList<LemmaSearchableParadigm> SearchableParadigms { get; set; }
    }
    

    public class LemmaSearchableParadigm
    {
        [Nested]
        public IList<LemmaInflectedForm> LemmaInflectedForms { get; set; }

    }

    public class LemmaInflectedForm
    {
        public string LeaveOut { get; set; }
        public IList<LemmaInflectedFormCategory> InflectedFormCategories { get; set; }
        public string InflectedFormFullForm { get; set; }
        public string InflectedFormCompactForm { get; set; }
    }

    public class LemmaInflectedFormCategory
    {
        public string InfCatNameDan { get; set; }
        public string InfCatNameGyl { get; set; }
        public string InfCatNameEng { get; set; }
        public string InfCatNameLat { get; set; }
        public string InfCatShortNameDan { get; set; }
        public string InfCatShortNameGyl { get; set; }
        public string InfCatShortNameEng { get; set; }
        public string InfCatShortNameLat { get; set; }
    }

    public class LemmaTablePresentation
    {
        public IList<LemmaTpRow> LemmaTpRows { get; set; }
    }
    public class LemmaTpRow
    {
        public IList<LemmaTpRowCells> LemmaTpRowCellses { get; set; }
    }
    public class LemmaTpRowCells
    {
        public string CellType { get; set; }
        public string CellName { get; set; }

    }

    public class LemmaUsage
    {
        public string Usage;
    }

    public class LemmaPronuciatetionAll
    {
        public IList<LemmaPronuciatetionVariant> ProVariants { get; set; }
    }
    public class LemmaPronuciatetionVariant
    {
        public IList<LemmaVariantDescription> VariantDescriptions { get; set; }
        public IList<LemmaPronunciation> LemmaPronunciations { get; set; }
    }
    public class LemmaVariantDescription
    {
        public string TechLang;
        public string VariantsDescription;
        public string LemmaProDescription;
        public IList<LemmaLangVariant> LangVariants { get; set; }
    }
    public class LemmaLangVariant
    {
        public string LangVariant;
    }

    public class LemmaPronunciation
    {
        public string SoundFile { get; set; }
        public string IPA { get; set; }
        public string IPApart { get; set; }
        public string Stress { get; set; }
    }

    public class LemmaIllustration
    {
        public IList<LemmaIllustrationFile> IllustrationFiles { get; set; }
    }
    public class LemmaIllustrationFile
    {
        public string IllFileType { get; set; }
        public string IllFileRef { get; set; }
    }

    public class LemmaAbbreviationFor
    {
        public IList<LemmablindRef> LemmablindRefs { get; set; }
        public IList<LemmaAbbrevationRef> AbbrevationRefs { get; set; }
    }

    public class LemmablindRef
    {
        public string LemmaBlindRef;
    }

    public class LemmaAbbrevationRef
    {
        public string AbbDescRef;
        public string AbbPos;
        public string AbbRef;
    }
}
