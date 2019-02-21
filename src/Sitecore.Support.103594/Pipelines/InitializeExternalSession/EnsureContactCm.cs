using Sitecore.Analytics.Configuration;
using Sitecore.Analytics.Data;
using Sitecore.Analytics.Model;
using Sitecore.Analytics.Pipelines.InitializeExternalSession;
using Sitecore.Analytics.Tracking;
using Sitecore.Diagnostics;
using System;

namespace Sitecore.Support.Analytics.Pipelines.InitializeExternalSession
{
  public class EnsureContactCm: Sitecore.Analytics.Pipelines.InitializeExternalSession.EnsureContactCm
  {
    public override void Process(InitializeExternalSessionArgs args)
    {
      ContactRepositoryBase repository;
      LeaseOwner owner;
      Assert.ArgumentNotNull(args, "args");
      Assert.IsNotNull(args.Session, "args.Session is not set.");
      if (args.Session.Contact == null)
      {
        Contact contact;
        Guid contactId = base.GetContactId(args);
        repository = this.GetContactRepository();
        owner = new LeaseOwner(AnalyticsSettings.ClusterName, LeaseOwnerType.WebCluster);
        if (contactId != Guid.Empty)
        {
          bool flag;
          contact = base.LoadContactWithRetries(args, repository, contactId, (cId, lease) => repository.TryLoadContact(cId, owner, lease), out flag);
        }
        else
        {
          contact = this.CreateContact(args, repository, out contactId);
        }
        args.Session.Contact = contact;
        args.IsContactLockPrivate = true;
        args.Owner = owner;
        if (args.Session.Device != null)
        {
          args.Session.Device.LastKnownContactId = new Guid?(contactId);
        }
      }
    }


  }
}
