using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.SqlCommandText
{
    /// <summary>
    /// 电销sql 处理
    /// </summary>
    public class CallLogSqlCommandText
    {
        /// <summary>
        /// 查询 备注报表
        /// </summary>
        public const string QueryCallRemarkReportSql = @"select * from
(
select count(id) totalCalls from ofCallLog with(nolock)
where DateDiff(mm,createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1
) a
,
(
select count(c.id) normalTotalCalls from ofCallLog c with(nolock)
left join ofUser u on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level in (10001, 10007) --普通会员和万象城会员,
) b
,
(
select count(c.id) normalPickCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level in (10001, 10007) --普通会员和万象城会员, pick
) bb
,
(
select count(c.id) vipTotalCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level not in (10001, 10007) --vip
) c
,
(
select count(c.id) vipPickCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level not in (10001, 10007) --vip pick
) cc
,
(
select count(c.id) vipHJTotalCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level= 10002 --黄金VIP 
) d
,
(
select count(c.id) vipHJPickCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level = 10002 --黄金VIP pick
) dd
,
(
select count(c.id) vipPTTotalCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level= 10007 --普通玩家 
) e
,
(
select count(c.id) vipPTPickCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level = 10007 --普通玩家 pick
) ee
,
(
select count(c.id) vipZSTotalCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level= 10004 --钻石VIP 
) f
,
(
select count(c.id) vipZSPickCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level = 10004 --钻石VIP pick
) ff
,
(
select count(c.id) vipTYTotalCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level= 10005 --特邀VIP
) g
,
(
select count(c.id) vipTYPickCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level = 10005 --特邀VIP pick
) gg
,
(
select count(c.id) vipZZTotalCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level= 10025 --至尊VIP
) h
,
(
select count(c.id) vipZZPickCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level = 10025 --至尊VIP pick
) hh
,
(
select count(c.id) vipJXTotalCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level= 10012 --见习VIP
) i
,
(
select count(c.id) vipJXPickCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level = 10012 --见习VIP pick
) ii
,
(
select count(c.id) vipBYINTotalCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level= 10027 --白银VIP
) j
,
(
select count(c.id) vipBYINPickCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0} and contactStatus=1 and u.level = 10027 --白银VIP pick
) jj
,
(
select count(c.id) vipBYTotalCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level= 10028 --白玉VIP
) k
,
(
select count(c.id) vipBYPickCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level = 10028 --白玉VIP pick
) kk
,
(
select count(c.id) vipWXTotalCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level= 10001 --万象会员
) l
,
(
select count(c.id) vipWXPickCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level = 10001 --万象会员 pick
) ll
,
(
select count(c.id) vipHCTotalCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level= 10026 --核查级别
) m
,
(
select count(c.id) vipHCPickCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0} and contactStatus=1 and u.level = 10026 --核查级别 pick
) mm
,
(
select count(c.id) vipBJTotalCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0} and contactStatus>-1 and u.level= 10003 --铂金VIP
) n
,
(
select count(c.id) vipBJPickCalls from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level = 10003 --铂金VIP pick
) nn
,
(
select count(distinct account) totalCallsV from ofCallLog with(nolock)
where DateDiff(mm,createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1
) av
,
(
select count(distinct c.account) normalTotalCallsV from ofCallLog c with(nolock)
left join ofUser u on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level in (10001, 10007) --普通会员和万象城会员,
) bv
,
(
select count(distinct c.account) normalPickCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level in (10001, 10007) --普通会员和万象城会员, pick
) bbv
,
(
select count(distinct c.account) vipTotalCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level not in (10001, 10007) --vip
) cv
,
(
select count(distinct c.account) vipPickCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level not in (10001, 10007) --vip pick
) ccv
,
(
select count(distinct c.account) vipHJTotalCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level= 10002 --黄金VIP 
) dv
,
(
select count(distinct c.account) vipHJPickCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level = 10002 --黄金VIP pick
) ddv
,
(
select count(distinct c.account) vipPTTotalCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level= 10007 --普通玩家 
) ev
,
(
select count(distinct c.account) vipPTPickCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level = 10007 --普通玩家 pick
) eev
,
(
select count(distinct c.account) vipZSTotalCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level= 10004 --钻石VIP 
) fv
,
(
select count(distinct c.account) vipZSPickCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level = 10004 --钻石VIP pick
) ffv
,
(
select count(distinct c.account) vipTYTotalCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level= 10005 --特邀VIP
) gv
,
(
select count(distinct c.account) vipTYPickCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level = 10005 --特邀VIP pick
) ggv
,
(
select count(distinct c.account) vipZZTotalCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level= 10025 --至尊VIP
) hv
,
(
select count(distinct c.account) vipZZPickCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level = 10025 --至尊VIP pick
) hhv
,
(
select count(distinct c.account) vipJXTotalCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level= 10012 --见习VIP
) iv
,
(
select count(distinct c.account) vipJXPickCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level = 10012 --见习VIP pick
) iiv
,
(
select count(distinct c.account) vipBYINTotalCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level= 10027 --白银VIP
) jv
,
(
select count(distinct c.account) vipBYINPickCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0} and contactStatus=1 and u.level = 10027 --白银VIP pick
) jjv
,
(
select count(distinct c.account) vipBYTotalCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level= 10028 --白玉VIP
) kv
,
(
select count(distinct c.account) vipBYPickCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level = 10028 --白玉VIP pick
) kkv
,
(
select count(distinct c.account) vipWXTotalCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level= 10001 --万象会员
) lv
,
(
select count(distinct c.account) vipWXPickCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level = 10001 --万象会员 pick
) llv
,
(
select count(distinct c.account) vipHCTotalCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus>-1 and u.level= 10026 --核查级别
) mv
,
(
select count(distinct c.account) vipHCPickCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0} and contactStatus=1 and u.level = 10026 --核查级别 pick
) mmv
,
(
select count(distinct c.account) vipBJTotalCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0} and contactStatus>-1 and u.level= 10003 --铂金VIP
) nv
,
(
select count( distinct c.account) vipBJPickCallsV from ofCallLog c with(nolock)
left join ofUser u with(nolock) on c.account = u.account
where DateDiff(mm,c.createtime,DATEADD(hour,8,getutcdate()))={0}  and contactStatus=1 and u.level = 10003 --铂金VIP pick
) nnv ";


        /// <summary>
        /// 查询符合条件的，备注记录数
        /// </summary>
        public const string QueryCallLogCountSql = @"SELECT count(*) cnt FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.createtime desc)AS Row, * from (
	select a.*,u.level from
	(
		SELECT  t1.account, t1.comment,t1.createtime,t1.createperson,t1.calltime
		FROM         
		ofCallLog AS t1 with(nolock) RIGHT OUTER JOIN
							  (SELECT     account, max(createtime) AS C
								FROM          ofCallLog with(nolock)
								where createtime >=@starttime and createtime<@endtime {0}
								GROUP BY account) AS t2 ON t1.account = t2.account AND t1.createtime = t2.C
	)a
	left join  ofUser u with(nolock) on a.account = u.account
 ) T
where 1=1 {1}
) 
TT
OPTION (RECOMPILE) ";


        /// <summary>
        /// 查询符合条件的，备注记录分页
        /// </summary>
        public const string QueryCallLogsSql = @"SELECT TT.* FROM 
( 
SELECT ROW_NUMBER() OVER 
(order by T.createtime desc)AS Row, * from 
(
	select a.*,b.amount,u.id uid,u.level,ug.levels from (
		SELECT  t1.account, t1.comment,t1.createtime,t1.createperson,t1.contactStatus,
		(select top 1 createtime from ofCallLog with(nolock) where account=t1.account and status=1 order by createtime desc) deleverTime,
		(select top 1 ISNULL(orderendtime,reviewtime) reviewtime  from ofDeposit with(nolock) where account=t1.account and status=1 order by ISNULL(orderendtime,reviewtime) desc) reviewtime

		FROM         
		ofCallLog AS t1 with(nolock) RIGHT OUTER JOIN
							  (SELECT     account, max(createtime) AS C
								FROM          ofCallLog with(nolock)
								where createtime >=@starttime and createtime<@endtime {0}
								GROUP BY account) AS t2 ON t1.account = t2.account AND t1.createtime = t2.C
	) a
	left join (
		select account,sum(amount) amount  
		from ofDeposit with(nolock) 
		where  status=1 and requesttime>'2020-01-01' and  ISNULL(orderendtime,reviewtime) >=@starttime and ISNULL(orderendtime,reviewtime)<@endtime
		group by account
	)b on a.account=b.account
	left join ofUser u with(nolock) on a.account=u.account
	left join ofUserGroup ug with(nolock) on u.level=ug.levelnumber
 ) T
where 1=1 {1}
) 
TT
WHERE TT.Row between @startIndex and @endIndex 
OPTION (RECOMPILE) ";

    }
}
