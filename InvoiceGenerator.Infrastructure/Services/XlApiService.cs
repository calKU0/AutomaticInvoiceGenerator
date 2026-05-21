using cdn_api;
using InvoiceGenerator.Contracts.Data.Enums;
using InvoiceGenerator.Contracts.DTOs;
using InvoiceGenerator.Contracts.Models;
using InvoiceGenerator.Contracts.Services;
using InvoiceGenerator.Contracts.Settings;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;

namespace InvoiceGenerator.Infrastructure.Services
{
    public class XlApiService : IXlApiService
    {
        [DllImport("ClaRUN.dll")]
        public static extern void AttachThreadToClarion(int _flag);

        private readonly XlApiSettings _settings;
        private int _xlSessionId = 0;

        public XlApiService(IOptions<XlApiSettings> settings)
        {
            _settings = settings.Value;
        }

        public void Login()
        {
            AttachThreadToClarion(1);

            XLLoginInfo_20251 xLLoginInfo = new XLLoginInfo_20251
            {
                Wersja = _settings.ApiVersion,
                ProgramID = _settings.ProgramName,
                Baza = _settings.Database,
                OpeIdent = _settings.Username,
                OpeHaslo = _settings.Password,
                TrybWsadowy = 1
            };

            int result = cdn_api.cdn_api.XLLogin(xLLoginInfo, ref _xlSessionId);

            if (result != 0)
            {
                throw new Exception($"Login error. Code: {result}");
            }
        }

        public void Logout()
        {
            AttachThreadToClarion(1);

            int result = cdn_api.cdn_api.XLLogout(_xlSessionId);

            if (result != 0)
            {
                throw new Exception($"Logout error. Code: {result}");
            }
        }

        public CreateInvoiceResult CreateInvoice(List<Order> orders)
        {
            int invoiceId = 0;
            try
            {
                AttachThreadToClarion(1);
                ManageTransaction(0);

                var orderHeader = orders.First();

                XLDokumentNagInfo_20251 invoice = new XLDokumentNagInfo_20251
                {
                    Wersja = _settings.ApiVersion,

                    Typ = orderHeader.DefaultDocumentType,

                    Forma = orderHeader.PaymentType,
                    Termin = orderHeader.PaymentDueDateClarion,

                    SposobDst = orderHeader.Courier,
                    RodzajCeny = orderHeader.PriceGroup,

                    ExpoNorm = orderHeader.ExpoNorm
                };

                if (orders.Count() == 1)
                {
                    invoice.ZamFirma = orderHeader.Company;
                    invoice.ZamTyp = orderHeader.Type;
                    invoice.ZamNumer = orderHeader.Id;
                    invoice.ZamLp = orderHeader.Lp;
                }
                else
                {
                    invoice.Akronim = orderHeader.ClientAcronym;
                    invoice.KnDTyp = orderHeader.ClientType;
                    invoice.KnDFirma = orderHeader.ClientCompany;
                    invoice.KnDNumer = orderHeader.ClientId;
                    invoice.KnDLp = orderHeader.ClientNo;

                    invoice.AdwTyp = orderHeader.AddressType;
                    invoice.AdwFirma = orderHeader.AddressCompany;
                    invoice.AdwNumer = orderHeader.AddressId;
                    invoice.AdwLp = orderHeader.AddressNo;
                    invoice.Opis = string.Join(" ", orders.DistinctBy(d => d.Description).Select(d => d.Description)).Trim();
                }

                int result = cdn_api.cdn_api.XLNowyDokument(_xlSessionId, ref invoiceId, invoice);
                if (result != 0)
                    throw new Exception(CheckError(result, XlApiFunctionCode.NowyDokument));

                foreach (var order in orders)
                {
                    foreach (var item in order.Items)
                    {
                        XLDokumentElemInfo_20251 invoiceElement = new XLDokumentElemInfo_20251
                        {
                            Wersja = _settings.ApiVersion,
                            TowarKod = item.Code,
                            Ilosc = item.Quantity.ToString(),
                            Cena = item.Price.ToString(),
                            CenaP = item.PriceBeforeDiscount.ToString(),
                            Waluta = item.Currency,

                            ZamFirma = order.Company,
                            ZamTyp = order.Type,
                            ZamLp = item.Lp,
                            ZamNumer = order.Id
                        };

                        result = cdn_api.cdn_api.XLDodajPozycje(invoiceId, invoiceElement);
                        if (result != 0)
                            throw new Exception($"Product: {item.Code} - {CheckError(result, XlApiFunctionCode.DodajPozycje)}");
                    }
                }

                XLZamkniecieDokumentuInfo_20251 closeInfo = new XLZamkniecieDokumentuInfo_20251
                {
                    Wersja = _settings.ApiVersion,
                    Tryb = 1
                };

                result = cdn_api.cdn_api.XLZamknijDokument(invoiceId, closeInfo);
                if (result != 0)
                    throw new Exception(CheckError(result, XlApiFunctionCode.ZamknijDokument));

                if (orders.Any(p => !string.IsNullOrEmpty(p.PackingRequirements)))
                {
                    var attribute = new AttributeObject
                    {
                        ClassName = "Wytyczne klienta do wysyłek",
                        Value = string.Join(" ", orders.DistinctBy(d => d.PackingRequirements).Select(d => d.PackingRequirements)).Trim(),
                        RefCompany = invoice.GIDFirma,
                        RefType = invoice.GIDTyp,
                        RefLp = invoice.GIDLp,
                        RefNumer = invoice.GIDNumer
                    };

                    result = AddAttribute(attribute);
                    if (result != 0)
                        throw new Exception($"Error adding attribute. Code: {result}");
                }

                ManageTransaction(1);

                return new CreateInvoiceResult
                {
                    InvoiceId = invoice.GIDNumer,
                    InvoiceType = invoice.GIDTyp,
                    InvoiceCompany = invoice.GIDFirma,
                    InvoiceLp = invoice.GIDLp
                };
            }
            catch
            {
                try
                {
                    if (ManageTransaction(3) == 1)
                        ManageTransaction(2);
                }
                catch { }

                throw;
            }
        }

        private int AddAttribute(AttributeObject attribute)
        {
            XLAtrybutInfo_20251 xLAtrybut = new XLAtrybutInfo_20251
            {
                Wersja = _settings.ApiVersion,
                Wartosc = attribute.Value,
                Klasa = attribute.ClassName,
                GIDNumer = attribute.RefNumer,
                GIDTyp = attribute.RefType,
                GIDLp = attribute.RefLp,
                GIDFirma = attribute.RefCompany
            };

            return cdn_api.cdn_api.XLDodajAtrybut(_xlSessionId, xLAtrybut);
        }

        private int ManageTransaction(int type)
        {
            XLTransakcjaInfo_20251 xLTransakcja = new XLTransakcjaInfo_20251
            {
                Wersja = _settings.ApiVersion,
                Tryb = type
            };

            int result = cdn_api.cdn_api.XLTransakcja(_xlSessionId, xLTransakcja);

            if (result != 1)
            {
                throw new Exception($"Transaction error. Code: {result}");
            }

            return xLTransakcja.TransakcjaAktywna;
        }

        private string CheckError(int errorCode, XlApiFunctionCode functionCode)
        {
            XLKomunikatInfo_20251 xLKomunikat = new XLKomunikatInfo_20251
            {
                Wersja = _settings.ApiVersion,
                Funkcja = (int)functionCode,
                Blad = errorCode,
                Tryb = 0
            };
            int result = cdn_api.cdn_api.XLOpisBledu(xLKomunikat);

            if (result == 0)
                return xLKomunikat.OpisBledu;
            else
                return $"Error attempting to check error. Code: {result}";
        }
    }
}
