import urllib.request
import re

url = "https://github.com/IT24104023/ClearToWork/actions"
req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})
html = urllib.request.urlopen(req).read().decode('utf-8')

runs = re.findall(r'href="/IT24104023/ClearToWork/actions/runs/(\d+)"', html)
run_ids = list(dict.fromkeys(runs))[:10]
print("Recent 10 workflow runs found on Actions page:")

for r in run_ids:
    run_url = f"https://github.com/IT24104023/ClearToWork/actions/runs/{r}"
    try:
        run_html = urllib.request.urlopen(urllib.request.Request(run_url, headers={'User-Agent': 'Mozilla/5.0'})).read().decode('utf-8')
        title_match = re.search(r'<title>(.*?)</title>', run_html)
        title = title_match.group(1) if title_match else "Unknown"
        
        status = "UNKNOWN"
        if "completed successfully" in run_html.lower() or "this run passed" in run_html.lower():
            status = "PASSED / GREEN [OK]"
        elif "this run failed" in run_html.lower() or "failed in" in run_html.lower():
            status = "FAILED [FAIL]"
        elif "in_progress" in run_html.lower() or "queued" in run_html.lower() or "in progress" in run_html.lower():
            status = "IN PROGRESS [RUNNING]"
            
        print(f"Run {r}: {status} | Title: {title[:80]}")
    except Exception as e:
        print(f"Run {r}: Error fetching ({e})")
