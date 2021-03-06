﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = NetOffice.WordApi;
using NetOffice.WordApi.Enums;
using System.IO;
using HYZK.FrameWork.DAL;
using DKLManager.Contract.Model;
using DKLManager.Contract;
using HYZK.Core.Config;
using HYZK.Core.Cache;
using DKLManager.Dal;
using HYZK.FrameWork.Common;
using HYZK.Account.Contract;
using DKLManager.Bll;




namespace OfficeDocGenerate                                                         //工作场所职业危害因素检测与评价
{
    public class CreateContractTestingAndEvaluation
    {
        ProjectContract model;
        string Header = "合同编号：                                 DKL/QB-11-05                      职业病危害因素检测与评价 ";
        string Footer = "北京德康莱健康安全科技股份有限公司                                              ";
        Word.Application m_wordApp;
        Word.Document m_doc;
        Word.Tools.CommonUtils m_utils;

        public CreateContractTestingAndEvaluation(ProjectContract Model)
        {
            this.model = Model;
            // start word and turn off msg boxes
            this.Header = model.ProjectNumber + "                   DKL/QB-28-02                 "; 
        }
        public List<string> CreateReportWord()
        {
            string appStatus;
            string strRet = null;
            List<string> appList = new List<string>();  //函数执行状态+文件名
            //测试使用的是fristpage

            using (m_wordApp = new Word.Application())
            {
                #region  定义类
                WriteFirstPage FirstPage = new OfficeDocGenerate.WriteFirstPage("ProjectNumber","CompaneName");
                #endregion
                m_wordApp.DisplayAlerts = WdAlertLevel.wdAlertsNone;
                m_utils = new Word.Tools.CommonUtils(m_wordApp);
                m_doc = m_wordApp.Documents.Add();

                //SetPage();
                WriteFirstPage(model.ProjectNumber, model.CompaneName, model.ContractDate.Year + "年" + model.ContractDate.Month + "月" + model.ContractDate.Day + "日");

          //      m_doc.Paragraphs.Last.Range.InsertBreak();
                WriteInstruction();
       //         m_doc.Paragraphs.Last.Range.InsertBreak();
                appStatus ="0";
                //WriteTestReport(ParmeterChemicalModels, physicalmodels, chemicalmodels, str);
                strRet = SaveFile();
                appList.Add(appStatus);
                appList.Add(strRet);
            }
            return appList;
        }
        private string SaveFile()
        {
            string fileName = "";
            string name = "合同模板样例" + DateTime.Now.ToFileTime();
            //      string documentFile = m_utils.File.Combine("c:\\", name, Word.Tools.DocumentFormat.Normal);
            string path = "d://DKLdownload";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string documentFile = m_utils.File.Combine(path, "合同模板样例", Word.Tools.DocumentFormat.Normal);
            if (FileSatus.FileIsOpen(documentFile) == 1)
            {
                //close the doc file
            }
            else
            {

                // fileName = documentFile;//.Replace(".docx", ".doc");
                fileName = documentFile.Replace(".docx", ".doc");     //如果出现文件损坏就用上边那个
                m_doc.SaveAs(fileName);
                m_wordApp.Quit();

                //if(!string.IsNullOrEmpty(fileName))
                //    System.Diagnostics.Process.Start(fileName);                
            }
            return fileName;
        }
        private void SetPage()
        {
            m_doc.PageSetup.PaperSize = WdPaperSize.wdPaperA4;
            m_doc.PageSetup.TopMargin = (float)70.875;
            m_doc.PageSetup.FooterDistance = (float)70;
            m_doc.PageSetup.LeftMargin = (float)56.7;
            m_doc.PageSetup.RightMargin = (float)56.7;
        }
        private void WriteFirstPage(string reportNum, string company, string reportDate)
        {
           
            m_wordApp.Selection.Font.Color = WdColor.wdColorBlack;
            m_wordApp.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;     
            m_wordApp.Selection.Font.Bold = 0;
            m_wordApp.Selection.Font.Size = 20;
            m_wordApp.Selection.TypeText("合同编号：" + model.ProjectNumber);              

           
            m_wordApp.Selection.Font.Bold = 5;
            m_wordApp.Selection.Font.Size = 40;
    //        m_wordApp.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            m_wordApp.Selection.TypeText("\r\n\r\n职业卫生技术服务合同");

            //测试结束
            m_wordApp.Selection.Font.Bold = 1;
            m_wordApp.Selection.Font.Size = 16;
            for (int i = 0; i < 7; ++i)
                m_wordApp.Selection.TypeText("\r\n");
            m_wordApp.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            m_wordApp.Selection.TypeText("\r\n项目名称：	" + model.ProjectName);

            m_wordApp.Selection.TypeText("\r\n委托方（甲方）：	" + model.CompaneName);
            m_wordApp.Selection.TypeText("\r\n受托方（乙方）：	" + "北京德康莱健康安全科技股份有限公司");

            //m_wordApp.Selection.TypeText("\r\n签订时间：	" + model.ContractDate);
            //m_wordApp.Selection.TypeText("\r\n签订地点：	" + company);
            //m_wordApp.Selection.TypeText("\r\n有效期限：	" + company);
            m_wordApp.Selection.TypeText("\r\n");

            m_doc.Paragraphs.Add();
            Word.Range range = m_doc.Paragraphs.First.Range;
         
            //Word.InlineShape shap = range.InlineShapes.AddPicture(@"NETofficeTest\NETofficeTest\bin\Debug\fv.png");

            m_doc.Sections[1].Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Text = Header;

            m_doc.Sections[1].Footers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Text = Footer;

            m_wordApp.Selection.TypeText("\r\n");
            Word.PageNumbers pns = m_wordApp.Selection.Sections[1].Headers[WdHeaderFooterIndex.wdHeaderFooterEvenPages].PageNumbers;
            pns.NumberStyle = WdPageNumberStyle.wdPageNumberStyleNumberInDash;
            pns.HeadingLevelForChapter = 0;
            pns.IncludeChapterNumber = false;
            pns.ChapterPageSeparator = WdSeparatorType.wdSeparatorHyphen;
            pns.RestartNumberingAtSection = false;
            pns.StartingNumber = 0;
            object pagenmbetal = WdPageNumberAlignment.wdAlignPageNumberRight;
            object first = true;
            m_wordApp.Selection.Sections[1].Footers[WdHeaderFooterIndex.wdHeaderFooterEvenPages].PageNumbers.Add(pagenmbetal, first);
            m_wordApp.Selection.Sections[1].Headers[WdHeaderFooterIndex.wdHeaderFooterEvenPages].PageNumbers.Add(pagenmbetal, first);


           // m_doc.ActiveWindow.View.SeekView = WdSeekView.wdSeekCurrentPageFooter;//转到页脚视图
           //m_wordApp.Selection.Paragraphs.Alignment = WdParagraphAlignment.wdAlignParagraphCenter; //整个页脚居中
           // object page = WdFieldType.wdFieldPage; //当前页码                   
           //m_wordApp.Selection.HeaderFooter.PageNumbers.RestartNumberingAtSection = true; // 重设起始页码
           //m_wordApp.Selection.HeaderFooter.PageNumbers.StartingNumber = 1; //设为1
           //m_wordApp.Selection.Fields.Add(m_wordApp.Selection.Range, page, 1, 1); // 加入 页码 字段
           // object alignment = WdPageNumberAlignment.wdAlignPageNumberCenter; //页码居中，注意必须在加入页码以后才可以用，否则出现异常
           //m_wordApp.Selection.HeaderFooter.PageNumbers[1].Alignment = WdPageNumberAlignment.wdAlignPageNumberCenter;
           //m_wordApp.ActiveWindow.View.SeekView = WdSeekView.wdSeekMainDocument; //转到正文




        }
        private void WriteInstruction()
        {
         
            m_wordApp.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            m_wordApp.Selection.Font.Bold = 1;
            m_wordApp.Selection.Font.Size = 22;
            m_wordApp.Selection.TypeText("\r\n\r\n\r\n职业卫生技术服务合同\r\n\r\n");
            m_wordApp.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            m_wordApp.Selection.Font.Size = 14;
            m_wordApp.Selection.Font.Bold = 0;
            m_wordApp.Selection.TypeText("本合同由"+model.CompaneName+" （下称甲方）委托北京德康莱健康安全科技股份有限公司（下称乙方）就"+model.CompaneName+""+model.ProjectName+"工作场所职业危害因素检测与评价技术服务工作，使甲方取得科学、准确的评价报告。双方经过平等协商，在真实、充分地表达各自意愿的基础上，根据《中华人民共和国合同法》的规定，达成以下协议，并由双方共同恪守。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 1;
            m_wordApp.Selection.Font.Size = 16;
            m_wordApp.Selection.TypeText("第一条 定义");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Size = 14;
            m_wordApp.Selection.Font.Bold = 0;
            m_wordApp.Selection.TypeText("本合同所使用的下述名词和术语，除非根据上下文有其它明显含义，其定义为：");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("1.1 “合同”  是指由甲方与乙方签订的"+model.CompaneName+"工作场所进行职业危害因素检测与评价技术服务合同，包括合同条款及在合同履行期间双方可能签订的备忘录或变更单。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("1.2 “报告”是指乙方为甲方提供的"+model.CompaneName+""+model.CompaneName+"工作场所职业危害因素检测与评价报告。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("1.3 “日”是指工作日。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("1.4 “规范”是指合同中列出的所有规则、规格书、标准和技术规范。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("1.5 “图纸”是指由甲方提供或乙方提供用于工作的全部图纸。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("1.6 “第三方”是指除甲方和乙方以外的任何一方。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 1;
            m_wordApp.Selection.Font.Size = 16;
            m_wordApp.Selection.TypeText("第二条 工作范围");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Size = 14;
            m_wordApp.Selection.Font.Bold = 0;
            m_wordApp.Selection.TypeText("乙方按甲方要求,依据国家《职业病防治法》相关法律法规及相关标准对"+model.CompaneName+""+model.ProjectName+"工作场所进行职业危害因素检测与评价，并编制相应评价文件,形成职业病危害因素检测与评价报告。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("上述报告应包括但不限于以下内容：");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("1 职业病危害因素检测与评价目的、依据、范围、内容和方法；");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("2 项目概况，包括建设地点、性质、规模、设计能力等；");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("3 项目工作场所存在的职业病危害因素的浓度或强度，并做出评价；");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("4 对工作场所存在的职业卫生问题提出有效的防护对策。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 1;
            m_wordApp.Selection.Font.Size = 16;
            m_wordApp.Selection.TypeText("第三条 甲方的责任和义务");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Size = 14;
            m_wordApp.Selection.Font.Bold = 0;
            m_wordApp.Selection.TypeText("3.1自合同签订之日起___日内，甲方向乙方提供可行性研究报告为参考资料。甲方应对提供资料的真实性、完整性负责。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("3.2 按合同规定向乙方支付合同价款。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("3.3 协助乙方完成现场调查、检测，并提供相应的保障及便利。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 1;
            m_wordApp.Selection.Font.Size = 16;
            m_wordApp.Selection.TypeText("第四条 乙方的责任和义务");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 0;
            m_wordApp.Selection.Font.Size = 14;
            m_wordApp.Selection.TypeText("4.1乙方应认真审查甲方提供的一切资料和文件，若发现有任何错误或不当，应及时书面通知甲方。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("4.2 乙方应按国家有关规范和行业标准向甲方提交能够反映该项目真实、准确、详细的职业病危害因素检测与评价资料，形成报告。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 1;
            m_wordApp.Selection.Font.Size = 16;
            m_wordApp.Selection.TypeText("第五条 工作进度和完工");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 0;
            m_wordApp.Selection.Font.Size = 14;
            m_wordApp.Selection.TypeText("5.1甲方向乙方交付本合同约定材料后，乙方应于30日内完成作业场所职业危害因素检测评价，出具评价报告。 乙方完成评价报告并通知甲方领取评价报告。.甲方应按照乙方通知的时间领取乙方的工作成果。甲方未按照乙方要求的时间接受工作成果的，视为甲方违约。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("5.2合同履行期间，在乙方的实际工作进度落后于计划进度的情况下，甲方有权要求乙方加班、增加人力或其他资源以保证按时提交报告。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("5.3 由于甲方未能按3.1款和3.3款要求及时提供资料，造成乙方实际工作进度落后于计划工作进度情况下，乙方出具评价报告的时间将顺延。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("5.4 乙方向甲方提供各"+model.ProjectName+"检测评价报告1份（含电子版）。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 1;
            m_wordApp.Selection.Font.Size = 16;
            m_wordApp.Selection.TypeText("第六条 合同价格");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 0;
            m_wordApp.Selection.Font.Size = 14;
            m_wordApp.Selection.TypeText("6.1甲方应向乙方支付技术服务费用" + (Convert.ToDouble(model.Money) / 10000).ToString("0.00") + "万元（大写：           万元）。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("6.2合同价格包含了乙方为履行合同所需的所有费用（包括检测费、评价费、成本、税金、管理费、申请备案的费用、利润以及为履行合同所发生的其它费用）。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 1;
            m_wordApp.Selection.Font.Size = 16;
            m_wordApp.Selection.TypeText("第七条 付款方式");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 0;
            m_wordApp.Selection.Font.Size = 14;
            m_wordApp.Selection.TypeText("7.1甲乙双方签订本合同后 5 日内，甲方应向乙方支付合同总额的" + model.PayRatioFirst + "%，即" + ((Convert.ToDouble(model.Money) / 10000) * (Convert.ToDouble(model.PayRatioFirst) / 100)).ToString("0.00") + "万元。报告交付前甲方应向乙方支付合同总额的" + (100 - Convert.ToInt16(model.PayRatioFirst)) + "%，即" + ((Convert.ToDouble(model.Money) / 10000) * ((Convert.ToDouble(100) - Convert.ToDouble(model.PayRatioFirst)) / 100)).ToString("0.00") + "万元。乙方在完成甲方现场检测并出具检测数据后表示已完成该项目50%的工作。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("7.2  乙方开户银行名称、地址和账号为：");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("收款单位：北京德康莱健康安全科技股份有限公司");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("帐    号：");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("汇 入 行：");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 1;
            m_wordApp.Selection.Font.Size = 16;
            m_wordApp.Selection.TypeText("第八条 保密");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 1;
            m_wordApp.Selection.Font.Size = 16;
            m_wordApp.Selection.TypeText("甲方：");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 0;
            m_wordApp.Selection.Font.Size = 14;
            m_wordApp.Selection.TypeText("8.1 保密内容：未经乙方书面同意，甲方不得将乙方提交的有关本项目的评价报告及其他与之相关的任何技术性文件、资料用于本合同目的之外的任何用途，否则，乙方将要求甲方另行支付相应费用。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("非为用于本合同之目的，未经乙方书面同意或根据法律之规定及政府相关部门的要求，甲方不得将上述材料向任何第三人公开或变相公开，包括甲方故意或过失将上述材料丢失致使上述材料为不特定第三人所知。否则，甲方的行为将视为对乙方权利的侵犯。在此种情况下，乙方有权要求甲方另行支付相关费用。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("8.2 涉密人员范围：甲方所有可能接触该评价报告的工作人员。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("8.3 保密期限：自甲方知悉上述约定的保密信息之日起至上述保密信息为公众所知之日止。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("8.4 泄密责任：甲方违反上述约定，应当承担相应赔偿责任。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 1;
            m_wordApp.Selection.Font.Size = 16;
            m_wordApp.Selection.TypeText("乙方：");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 0;
            m_wordApp.Selection.Font.Size = 14;
            m_wordApp.Selection.TypeText("8.5保密内容：乙方对甲方提供的资料仅用于编制该项目职业危害因素评价报告，不得用于除此以外的其他任何目的。但乙方依照国家法律、法规及相关政府部门的要求除外。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("8.6 涉密人员范围：乙方所有涉及该项目技术资料的人员。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("8.7 保密期限：自乙方知悉上述保密内容之日起至上述保密信息为公众所知之日止。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("8.8泄密责任：乙方违反本约定造成甲方经济损失，甲方有权依法要求赔偿。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 1;
            m_wordApp.Selection.Font.Size = 16;
            m_wordApp.Selection.TypeText("第九条  成果验收");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 0;
            m_wordApp.Selection.Font.Size = 14;
            m_wordApp.Selection.TypeText("9.1 乙方提交的技术服务工作成果形式为："+model.ProjectName+"工作场所职业危害因素检测与评价报告。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("9.3验收地点："+model.CompaneName+"");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 1;
            m_wordApp.Selection.Font.Size = 16;
            m_wordApp.Selection.TypeText("第十条 成果归属");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 0;
            m_wordApp.Selection.Font.Size = 14;
            m_wordApp.Selection.TypeText("乙方利用甲方提供的技术资料和工作条件完成的该项目评价报告书及其他相关技术资料，在甲方全部支付合同约定的服务费用前，其所有权归乙方完全所有。未经乙方同意，甲方不得以任何目的，以任何形式使用或许可他人使用。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("甲方支付合同约定的全部款项后，本合同涉及的该项目评价报告书及其他相关技术资料的使用权归甲方（但乙方有署名权）。同时，甲方对该项目评价报告书的权利仅限于为本合同的目的使用。甲方为本合同目的之外使用评价报告的，应经乙方同意并支付相关费用。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("该项目评价报告书的使用权利转归甲方后，乙方可以为自身的经营业务的需要，引用上述报告的内容而不需要预先告知甲方亦无须支付任何费用。同时，乙方可以为宣传自身业务的需要，以任何方式向任何第三人或不特定的人群表明，乙方曾经为甲方进行过上述工作。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 1;
            m_wordApp.Selection.Font.Size = 16;
            m_wordApp.Selection.TypeText("第十一条  终止");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 0;
            m_wordApp.Selection.Font.Size = 14;
            m_wordApp.Selection.TypeText("乙方向甲方提交报告且双方的债权债务结清后，本合同终止。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 1;
            m_wordApp.Selection.Font.Size = 16;
            m_wordApp.Selection.TypeText("第十二条  适用法律");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 0;
            m_wordApp.Selection.Font.Size = 14;
            m_wordApp.Selection.TypeText("合同的履行及解释适用《中华人民共和国合同法》及相关法律。");
            m_wordApp.Selection.TypeParagraph();       
            m_wordApp.Selection.Font.Bold = 1;
            m_wordApp.Selection.Font.Size = 16;
            m_wordApp.Selection.TypeText("第十三条  违约");
            m_wordApp.Selection.Font.Bold = 0;
            m_wordApp.Selection.Font.Size = 14;
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("13.1乙方未按合同约定时间完成工作的，甲方有权按下述规定从应付给乙方的合同款项中扣除该延期的违约金。经甲方同意延长履行期限的除外。甲方未能按约定日期提交相关材料的，乙方履行期限顺延。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("13.2 乙方未按约定日期完成工作的，每延期1日的违约金为合同总金额的0.5%。乙方承担的延期履行违约金以合同总金额的20%为上限。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("13.3甲方未按本合同7.1条规定支付首付款的，乙方有权拒绝赴现场工作。甲方超过30日仍未支付首付款的，乙方有权解除本合同，同时，要求甲方支付合同款项50%的违约金。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("13.4甲方未按照合同5.1条的约定，领取乙方提交的工作成果，视为甲方违约。经乙方提示，甲方仍不领取的，乙方有权解除本合同。在此种情况下，甲方已经支付的款项乙方不予退还，同时，甲方还应向乙方支付违约金，违约金的数额为合同金额的50%。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 1;
            m_wordApp.Selection.Font.Size = 16;
            m_wordApp.Selection.TypeText("第十四条  争议");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 0;
            m_wordApp.Selection.Font.Size = 14;
            m_wordApp.Selection.TypeText("14.1 对合同的解释或履行所引起的任何争议，双方首先应尽最大努力通过友好协商解决。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("14.2 如争议发生后协商不成，合同任何一方可将争议提交乙方所在地人民法院。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 1;
            m_wordApp.Selection.Font.Size = 16;
            m_wordApp.Selection.TypeText("第十五条  合同有效期");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 0;
            m_wordApp.Selection.Font.Size = 14;
            m_wordApp.Selection.TypeText("本合同自双方授权代表签字之日起生效，成果交付完毕、费用结算清楚后失效。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 1;
            m_wordApp.Selection.Font.Size = 16;
            m_wordApp.Selection.TypeText("第十六条  其它");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 0;
            m_wordApp.Selection.Font.Size = 14;
            m_wordApp.Selection.TypeText("16.1其它未尽事宜，由双方友好协商补充完善。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.TypeText("16.2合同一式四份，甲、乙双方各二份具有同等法律效力。");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 1;
            m_wordApp.Selection.Font.Size = 16;
            m_wordApp.Selection.TypeText("第十七条 联系");
            m_wordApp.Selection.TypeParagraph();
            m_wordApp.Selection.Font.Bold = 0;
            m_wordApp.Selection.Font.Size = 14;
            Word.Table basicInfo = m_doc.Tables.Add(m_wordApp.Selection.Range, 14, 4);  //-
            basicInfo.Borders.Enable = 1;
            basicInfo.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;
            basicInfo.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
            basicInfo.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
            //basicInfo.Columns[1].Width = 90;
            
            basicInfo.Rows[1].Height = 20;
            basicInfo.Cell(1, 1).Select();
            SetCellHeaderText("甲方:");
            basicInfo.Cell(1, 2).Select();
            SetCellHeaderText(model.CompaneName);
            basicInfo.Cell(1, 3).Select();
            SetCellHeaderText("乙方:");
            basicInfo.Cell(1, 4).Select();
            SetCellHeaderText("北京德康莱健康安全科技股份有限公司");
            basicInfo.Cell(2, 1).Select();
            SetCellHeaderText("法定代表人:");
            basicInfo.Cell(2, 2).Select();
            SetCellHeaderText("");
            basicInfo.Cell(2, 3).Select();
            SetCellHeaderText("法定代表人:");
            basicInfo.Cell(2, 4).Select();
            SetCellHeaderText("");
            basicInfo.Cell(3, 1).Select();
            SetCellHeaderText("代表人电话：");
            basicInfo.Cell(3, 2).Select();
            SetCellHeaderText("");
            basicInfo.Cell(3, 3).Select();
            SetCellHeaderText("代表人电话：");
            basicInfo.Cell(3, 4).Select();
            SetCellHeaderText("");
            basicInfo.Cell(4, 1).Select();
            SetCellHeaderText("传真：");
            basicInfo.Cell(4, 2).Select();
            SetCellHeaderText("");
            basicInfo.Cell(4, 3).Select();
            SetCellHeaderText("传真：");
            basicInfo.Cell(4, 4).Select();
            SetCellHeaderText("");
            basicInfo.Cell(5, 1).Select();
            SetCellHeaderText("地址：");
            basicInfo.Cell(5, 2).Select();
            SetCellHeaderText("");
            basicInfo.Cell(5, 3).Select();
            SetCellHeaderText("地址：");
            basicInfo.Cell(5, 4).Select();
            SetCellHeaderText("");
            basicInfo.Cell(6, 1).Select();
            SetCellHeaderText("联系人：");
            basicInfo.Cell(6, 2).Select();
            SetCellHeaderText("");
            basicInfo.Cell(6, 3).Select();
            SetCellHeaderText("联系人：");
            basicInfo.Cell(6, 4).Select();
            SetCellHeaderText("");
            basicInfo.Cell(7, 1).Select();
            SetCellHeaderText("电话：");
            basicInfo.Cell(7, 2).Select();
            SetCellHeaderText("");
            basicInfo.Cell(7, 3).Select();
            SetCellHeaderText("电话：");
            basicInfo.Cell(7, 4).Select();
            SetCellHeaderText("");
            basicInfo.Cell(8, 1).Select();
            SetCellHeaderText("邮编：");
            basicInfo.Cell(8, 2).Select();
            SetCellHeaderText("");
            basicInfo.Cell(8, 3).Select();
            SetCellHeaderText("邮编：");
            basicInfo.Cell(8, 4).Select();
            SetCellHeaderText("");
            basicInfo.Cell(9, 1).Select();
            SetCellHeaderText("邮箱：");
            basicInfo.Cell(9, 2).Select();
            SetCellHeaderText("");
            basicInfo.Cell(9, 3).Select();
            SetCellHeaderText("邮箱：");
            basicInfo.Cell(9, 4).Select();
            SetCellHeaderText("");
            basicInfo.Cell(10, 1).Select();
            SetCellHeaderText("授权人姓名：");
            basicInfo.Cell(10, 2).Select();
            SetCellHeaderText("");
            basicInfo.Cell(10, 3).Select();
            SetCellHeaderText("授权人姓名：");
            basicInfo.Cell(10, 4).Select();
            SetCellHeaderText("");
            basicInfo.Cell(11, 1).Select();
            SetCellHeaderText("职务：");
            basicInfo.Cell(11, 2).Select();
            SetCellHeaderText("");
            basicInfo.Cell(11, 3).Select();
            SetCellHeaderText("职务：");
            basicInfo.Cell(11, 4).Select();
            SetCellHeaderText("");
            basicInfo.Cell(12, 1).Select();
            SetCellHeaderText("授权代理人签名：");
            basicInfo.Cell(12, 2).Select();

            basicInfo.Cell(12, 3).Select();
            SetCellHeaderText("授权代理人签名：");
            basicInfo.Cell(12, 4).Select();

            basicInfo.Cell(13, 1).Select();
            SetCellHeaderText("单位盖章：");
            basicInfo.Cell(13, 2).Select();

            basicInfo.Cell(13, 3).Select();
            SetCellHeaderText("单位盖章：");
            basicInfo.Cell(13, 4).Select();
            basicInfo.Cell(14, 1).Select();
            SetCellHeaderText("签订日期：");
            basicInfo.Cell(14, 2).Select();
            SetCellHeaderText(model.ContractDate.Year + "年" + model.ContractDate.Month + "月" + model.ContractDate.Day + "日");
            basicInfo.Cell(14, 3).Select();
            SetCellHeaderText("签订日期：");
            basicInfo.Cell(14, 4).Select();
            SetCellHeaderText(model.ContractDate.Year + "年" + model.ContractDate.Month + "月" + model.ContractDate.Day + "日");
            m_wordApp.Selection.MoveDown();//光标下移，光标移出表格
            m_wordApp.Selection.MoveDown();//光标下移，光标移出表格
            m_wordApp.Selection.MoveDown();//光标下移，光标移出表格
            m_wordApp.Selection.MoveDown();//光标下移，光标移出表格
            m_wordApp.Selection.MoveDown();//光标下移，光标移出表格
            m_wordApp.Selection.MoveDown();//光标下移，光标移出表格


        }
        private void SetCellHeaderText(string text)
        {
            SetCellText(1, 12, text, WdParagraphAlignment.wdAlignParagraphCenter, WdCellVerticalAlignment.wdCellAlignVerticalCenter);
        }
        private void SetCellText(int bold, int size, string text, WdParagraphAlignment wAlign, WdCellVerticalAlignment vAlign)
        {
            m_wordApp.Selection.Cells.VerticalAlignment = vAlign;
            SetParaText(bold, size, text, wAlign);
        }
        private void SetParaText(int bold, int size, string text, WdParagraphAlignment wAlign)
        {
            m_wordApp.Selection.Font.Bold = bold;
            m_wordApp.Selection.Font.Color = WdColor.wdColorBlack;
            m_wordApp.Selection.Font.Size = size;
            m_wordApp.Selection.ParagraphFormat.Alignment = wAlign;
            m_wordApp.Selection.TypeText(text);
        }
        private void TypeText(int bold, int size, string text)
        {
            m_wordApp.Selection.Font.Bold = bold;
            m_wordApp.Selection.Font.Color = WdColor.wdColorBlack;
            m_wordApp.Selection.Font.Size = size;
            m_wordApp.Selection.TypeText(text);
        }
   

    }
}

