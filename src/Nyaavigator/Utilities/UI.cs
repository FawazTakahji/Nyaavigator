using System;
using System.Collections.Generic;
using Nyaavigator.Models;

namespace Nyaavigator.Utilities;

internal static class UI
{
    public static IList<Filter> GetFilters()
    {
        IList<Filter> filters =
        [
            new Filter("No Filter", 0),
            new Filter("No Remakes", 1),
            new Filter("Trusted Only", 2)
        ];
        return filters;
    }

    public static IList<Category> GetCategories()
    {
        IList<Category> categories =
        [
            new Category("All Categories", "0_0"),
            new Category("Anime", "1_0",
            [
                new Category("Anime Music Video", "Anime - AMV", "1_1"),
                new Category("English Translated", "Anime - ET", "1_2"),
                new Category("Non English Translated", "Anime - NET", "1_3"),
                new Category("Raw", "Anime - Raw", "1_4")
            ]),
            new Category("Audio", "2_0",
            [
                new Category("Lossless", "Audio - Lossless", "2_1"),
                new Category("Lossy", "Audio - Lossy", "2_2")
            ]),
            new Category("Literature", "3_0",
            [
                new Category("English Translated", "Literature - ET", "3_1"),
                new Category("Non English Translated", "Literature - NET", "3_2"),
                new Category("Raw", "Literature - Raw", "3_3")
            ]),
            new Category("Live Action", "4_0",
            [
                new Category("English Translated", "Live Action - ET", "4_1"),
                new Category("Idol/Promotional Video", "Live Action - Idol/PV", "4_2"),
                new Category("Non English Translated", "Live Action - NET", "4_3"),
                new Category("Raw", "Live Action - Raw", "4_4")
            ]),
            new Category("Pictures", "5_0",
            [
                new Category("Graphics", "Pictures - Graphics", "5_1"),
                new Category("Photos", "Pictures - Photos", "5_2")
            ]),
            new Category("Software", "6_0",
            [
                new Category("Applications", "Software - Apps", "6_1"),
                new Category("Games", "Software - Games", "6_2")
            ])
        ];
        return categories;
    }

#if DEBUG
    public static TorrentInfo GetFakeInfo()
    {
        TorrentInfo info = new()
        {
            Name = "Test",
            Category = "1_1",
            Size = "1 GiB",
            Date = DateTime.Now,
            Seeders = "100",
            Leechers = "100",
            Downloads = "100",
            Hash = "dhnj2738hd232h13723",
            Information = "https://example.com",
            Submitter = new User("Example", "user/Example"),
            Items =
            [
                new ListItem
                {
                    Name = "Folder",
                    IsFolder = true,
                    Children =
                    [
                        new ListItem
                        {
                            Name = "Folder 2",
                            IsFolder = true,
                            Children =
                            [
                                new ListItem
                                {
                                    Name = "File",
                                    Size = "1 GiB"
                                }
                            ]
                        },
                        new ListItem
                        {
                            Name = "File",
                            Size = "1 GiB"
                        }
                    ]
                }
            ],
            Comments =
            [
                new Comment
                {
                    Date = DateTime.Today.AddDays(-4),
                    User = new User("Uploader", "user/Uploader"),
                    Text = "Uploader Text",
                    IsUploader = true
                },
                new Comment
                {
                    Date = DateTime.Today.AddDays(-3),
                    User = new User("Trusted", "user/Trusted")
                    {
                        IsTrusted = true
                    },
                    Text = "Trusted Text"
                },
                new Comment
                {
                    Date = DateTime.Today.AddDays(-2),
                    User = new User("Banned", "user/Banned")
                    {
                        IsBanned = true
                    },
                    Text = "Banned Text"
                },
                new Comment
                {
                    Date = DateTime.Today.AddDays(-1),
                    User = new User("Admin", "user/Admin")
                    {
                        IsAdmin = true
                    },
                    Text = "Admin Text"
                },
                new Comment
                {
                    Date = DateTime.Today.AddDays(-1),
                    EditedDate = DateTime.Today.AddDays(-1).AddHours(6),
                    User = new User("Edited", "user/Edited"),
                    Text = "Edited Text",
                    IsEdited = true
                },
                new Comment
                {
                    Date = DateTime.Now,
                    EditedDate = DateTime.Now.AddHours(6),
                    User = new User("All", "user/All")
                    {
                        IsTrusted = true,
                        IsBanned = true,
                        IsAdmin = true
                    },
                    Text = "All Text",
                    IsUploader = true,
                    IsEdited = true
                }
            ],
            Description = """
                          Vesta filis quod ipse legendo flammas et
                          
                          Redeuntem videtur
                          
                          Lorem markdownum aconita patrias vidisset fatemur habet duro, abit. Haec ipse
                          fido me hinc ad canenda, fuit fides, et.
                          
                          Quid capiti **obrutaque te Ascaniumque** cognoscendo crediderim nefas, aut
                          matris latronis: fronde si certos Gorgoneum. Luctus absensauxilium, Argo mihi
                          inque: heros Lelegeides, Ausoniae aspera curribus silet, tenet? Album vicinia
                          omnes suprema.
                          
                          Dominae nova. Tunc sibi nymphae, nil quae contingere acceptaque quoque *in fides
                          finibus* corpora corpus. Non intima: nunc nox dentes nec alto principiis vitam
                          inde; sibi isque, peto dederat est.
                          
                          1. Aquilone lugeat
                          2. Natam est animoque leto positis latus tamen
                          3. Ad tamen idem templo
                          4. Ligno quid aureus
                          5. De vixque
                          
                          Omnis virgineosque deseret viriles annis pater auresque
                          
                          Tu removit aliquid credensque undae oris furit iam genus ilia templis nostro.
                          Terras solutis et instanti exilium sinistra puppes, cognatumque canos, taurus
                          totumque dicit. Ad contulit quaecumque orbis frena frustraque in famaque parenti
                          femineae oriens quod. Aura *harena territus*; erat nomina: amplexu: secutis ille
                          Pleionesque recepto.
                          
                          > Inmitis nata ire **et spatio** procubuit quam, ibi qui conubia, respiceret
                          > centum crimine nive ursi nulla habet canentis? Iunxit ab desunt silvamque
                          > ungues; vim dis sanguine aut virginea Iove super venitis! Et et saliente
                          > Pontum cum lapsus fixit obstat est ruit. Poplite mortis Herculeae referri
                          > hortator: num mea secedere [alis](http://quidaquas.com/ponensquenec.php), est.
                          > Persea iubentem fontibus magis, *qui nec erat* quodque, nunc.
                          
                          Non relinquant cornu Vestae omne hostibus arvo fingi nec obortis, iugo. Traxit
                          cum paruit salve gentis, cura quoque et et dum illa *mediam est* unde unam
                          torsi. Desunt sinitis faciat seque repetita nataeque; pater stagni egressus
                          pedibus?
                          
                              if (450662 != editor) {
                                  wysiwygKeyloggerDfs(https_data_page, down, 1);
                                  lampEHorizontal += webmail;
                              } else {
                                  base(memory(unitWiRecursion, 1), 35, 3);
                              }
                              if (ftpCodeMirrored >= toolbar_peopleware_browser) {
                                  dns *= excel;
                              } else {
                                  wimax_service = ergonomicsDriveText;
                                  tweak_system.ics_repeater += browserNet;
                              }
                              thermistorVersionRepository += heuristic(sync_sdram_port * appletSpoolBoot,
                                      skin, ethernet) + 45 + bitmapFileIntellectual(6, -1, 4);
                          
                          Ponit suum quinos eventus tempora venit tumidi timidusque matrona descendunt
                          proles illis tamen totaeque fatum culmine armentaque rear. Cum hic iram reddita
                          habuisse mutatis diligitur illum modo ave vellera videt timet, pectus.
                          """
        };
        return info;
    }
#endif
}